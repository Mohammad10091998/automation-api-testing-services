using System.Collections;
using System.Dynamic;
using System.Linq.Dynamic.Core;
using System.Security;
using System.Text.Json.Nodes;
using APITestingService;
using ModelsLibrary;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class GenerateTestObjectHelper
{
    //This is the currentIndex during recurrssion of creating different test objects
    public static int currentIndex;
    public static async Task<List<TestPayloadInfo>> CreateTestObjectsFromExpando(dynamic input)
    {
        try
        {
            if (input is ExpandoObject)
            {
                LinkedList<Node> propertyNodes = new LinkedList<Node>();
                Type objectType = CreateAbstractClassType(input, propertyNodes);
                //All positive value typed object
                var typedObject = CreateDifferentTestObject(objectType, input, null, null, -1);
                List<TestPayloadInfo> testObjects = GenerateUniqueTestObjects(propertyNodes, objectType, input);

                //Adding positive scenario given by user
                TestPayloadInfo posTestPayloadInfo = new TestPayloadInfo
                {
                    TestObject = typedObject,
                };
                testObjects.Add(posTestPayloadInfo);
                return testObjects;
            }

            throw new Exception("Invalid Data: Input is not of ExpandoObject type");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public static Type CreateAbstractClassType(dynamic input, LinkedList<Node> list)
    {
        List<DynamicProperty> props = new List<DynamicProperty>();

        if (input == null)
        {
            return typeof(object);
        }
        if (!(input is ExpandoObject))
        {
            return input.GetType();
        }

        else
        {
            foreach (var expando in (IDictionary<string, object>)input)
            {
                Type value;
                if (expando.Value is IList)
                {
                    if (((IList)expando.Value).Count == 0)
                        value = typeof(List<object>);
                    else
                    {
                        var internalType = CreateAbstractClassType(((IList)expando.Value)[0], list);
                        value = new List<object>().Cast(internalType).ToList(internalType).GetType();
                    }

                }
                else
                {
                    value = CreateAbstractClassType(expando.Value, list);
                }
                AddNodeInTheList(expando.Key, list, expando.Value);
                props.Add(new DynamicProperty(expando.Key, value));
            }
        }

        var type = DynamicClassFactory.CreateType(props);
        return type;
    }

    public static object CreateDifferentTestObject(Type type, dynamic input, string updatingKey, object updatingValue, int propertyIndex)
    {
        if (!(input is ExpandoObject))
        {
            return Convert.ChangeType(input, type);
        }
        object obj = Activator.CreateInstance(type);

        var typeProps = type.GetProperties().ToDictionary(c => c.Name);

        foreach (var expando in (IDictionary<string, object>)input)
        {
            if (typeProps.ContainsKey(expando.Key) &&
                expando.Value != null && (expando.Value.GetType().Name != "DBNull" || expando.Value != DBNull.Value))
            {
                object val;
                var propInfo = typeProps[expando.Key];
                if (expando.Value is ExpandoObject)
                {
                    var propType = propInfo.PropertyType;
                    val = CreateDifferentTestObject(propType, expando.Value, updatingKey, updatingValue, propertyIndex);
                }
                else if (expando.Value is IList)
                {
                    var internalType = propInfo.PropertyType.GenericTypeArguments.FirstOrDefault() ?? typeof(object);
                    var temp = (IList)expando.Value;
                    var newList = new List<object>().Cast(internalType).ToList(internalType);
                    for (int i = 0; i < temp.Count; i++)
                    {
                        var child = CreateDifferentTestObject(internalType, temp[i], updatingKey, updatingValue, propertyIndex);
                        newList.Add(child);
                    };
                    val = newList;
                }
                else
                {
                    val = expando.Value;
                    if (expando.Key == updatingKey)
                    {
                        if (propertyIndex == currentIndex)
                        {
                            if (propInfo.PropertyType == typeof(string) && updatingValue is char charValue)
                            {
                                // Convert char to string
                                updatingValue = charValue.ToString();
                            }
                            val = updatingValue;
                        }

                        currentIndex++;

                    }
                }
                propInfo.SetValue(obj, val, null);
            }
        }

        return obj;
    }

    private static IEnumerable Cast(this IEnumerable self, Type innerType)
    {
        var methodInfo = typeof(Enumerable).GetMethod("Cast");
        var genericMethod = methodInfo.MakeGenericMethod(innerType);
        return genericMethod.Invoke(null, new[] { self }) as IEnumerable;
    }

    private static IList ToList(this IEnumerable self, Type innerType)
    {
        var methodInfo = typeof(Enumerable).GetMethod("ToList");
        var genericMethod = methodInfo.MakeGenericMethod(innerType);
        return genericMethod.Invoke(null, new[] { self }) as IList;
    }
    private static void AddNodeInTheList(string key, LinkedList<Node> list, object valueOfProp)
    {
        string parsedType = ParseType(valueOfProp);

        if (!string.IsNullOrEmpty(parsedType))
        {
            Node node = new Node(key, parsedType);
            list.AddLast(node);
        }
    }
    private static string ParseType(object value)
    {
        if (int.TryParse(value.ToString(), out _))
        {
            return "int";
        }
        else if (long.TryParse(value.ToString(), out _))
        {
            return "long";
        }
        else if (double.TryParse(value.ToString(), out _))
        {
            return "double";
        }
        else if (float.TryParse(value.ToString(), out _))
        {
            return "float";
        }
        else if (decimal.TryParse(value.ToString(), out _))
        {
            return "decimal";
        }
        else if (bool.TryParse(value.ToString(), out _))
        {
            return "bool";
        }
        else if (DateTime.TryParse(value.ToString(), out _))
        {
            return "datetime";
        }
        else if (Guid.TryParse(value.ToString(), out _))
        {
            return "guid";
        }
        else if (char.TryParse(value.ToString(), out _))
        {
            return "char";
        }
        else if (value.GetType() == typeof(string))
        {
            return "string";
        }

        return string.Empty;
    }
    public static List<TestPayloadInfo> GenerateUniqueTestObjects(LinkedList<Node> propertyNodes, Type objectType, dynamic input)
    {
        List<TestPayloadInfo> uniqueTestObjects = new List<TestPayloadInfo>();

        Dictionary<string, int> keyCountMap = new Dictionary<string, int>();

        foreach (var propertyNode in propertyNodes)
        {
            if (keyCountMap.TryGetValue(propertyNode.Key, out int propertyIndex))
            {
                propertyIndex++;
                keyCountMap[propertyNode.Key] = propertyIndex;
            }
            else
            {
                keyCountMap.Add(propertyNode.Key, 0);
                propertyIndex = 0;
            }

            if (propertyNode.PropertyType == "int")
            {
                foreach (var value in TestDataValues.IntegerValues)
                {
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, value, propertyIndex);
                    var testPayloadInfo = CreateTestPayloadInfo(testObject, propertyNode.Key, value.ToString(), propertyNode.PropertyType);
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
            if (propertyNode.PropertyType == "long")
            {
                foreach (var value in TestDataValues.LongValues)
                {
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, value, propertyIndex);
                    var testPayloadInfo = CreateTestPayloadInfo(testObject, propertyNode.Key, value.ToString(), propertyNode.PropertyType);
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
            else if (propertyNode.PropertyType == "string")
            {
                foreach (var value in TestDataValues.StringValues)
                {
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, value, propertyIndex);
                    var testPayloadInfo = CreateTestPayloadInfo(testObject, propertyNode.Key, value?.ToString(), propertyNode.PropertyType);
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
            else if (propertyNode.PropertyType == "double")
            {
                foreach (var value in TestDataValues.DoubleValues)
                {
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, value, propertyIndex);
                    var testPayloadInfo = CreateTestPayloadInfo(testObject, propertyNode.Key, value.ToString(), propertyNode.PropertyType);
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
            else if (propertyNode.PropertyType == "float")
            {
                foreach (var value in TestDataValues.FloatValues)
                {
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, value, propertyIndex);
                    var testPayloadInfo = CreateTestPayloadInfo(testObject, propertyNode.Key, value.ToString(), propertyNode.PropertyType);
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
            else if (propertyNode.PropertyType == "decimal")
            {
                foreach (var value in TestDataValues.DecimalValues)
                {
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, value, propertyIndex);
                    var testPayloadInfo = CreateTestPayloadInfo(testObject, propertyNode.Key, value.ToString(), propertyNode.PropertyType);
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
            else if (propertyNode.PropertyType == "bool")
            {
                foreach (var value in TestDataValues.BoolValues)
                {
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, value, propertyIndex);
                    var testPayloadInfo = CreateTestPayloadInfo(testObject, propertyNode.Key, value.ToString(), propertyNode.PropertyType);
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
            else if (propertyNode.PropertyType == "datetime")
            {
                foreach (var value in TestDataValues.DateTimeValues)
                {
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, value, propertyIndex);
                    var testPayloadInfo = CreateTestPayloadInfo(testObject, propertyNode.Key, value.ToString("yyyy-MM-ddTHH:mm:ss"), propertyNode.PropertyType);
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
            else if (propertyNode.PropertyType == "guid")
            {
                foreach (var value in TestDataValues.GuidValues)
                {
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, value, propertyIndex);
                    var testPayloadInfo = CreateTestPayloadInfo(testObject, propertyNode.Key, value.ToString(), propertyNode.PropertyType);
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
            else if (propertyNode.PropertyType == "char")
            {
                foreach (var value in TestDataValues.CharValues)
                {
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, value, propertyIndex);
                    var testPayloadInfo = CreateTestPayloadInfo(testObject, propertyNode.Key, value.ToString(), propertyNode.PropertyType);
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
            // Add more conditions for other data types as needed
        }

        return uniqueTestObjects;
    }

    private static TestPayloadInfo CreateTestPayloadInfo(object testObject, string negativePropertyName, object negativePropertyValue, string negativePropertyType)
    {
        return new TestPayloadInfo
        {
            TestObject = testObject,
            NegativePropertyName = negativePropertyName,
            NegativePropertyValue = negativePropertyValue?.ToString(),
            NegativePropertyType = negativePropertyType
        };
    }
    public static string GenerateURLFromParams(string apiUrl, string key, string value, List<KeyValue> @params, int index)
    {
        if (key != null && value != null)
        {
            apiUrl = $"{apiUrl}?{key}={value}";
        }
        for (int ind = 0; ind < @params.Count; ind++)
        {
            if (ind == index) continue;
            var param = @params[ind];
            if (apiUrl.Contains("?"))
            {
                apiUrl = $"{apiUrl}&{param.Key}={param.Value}";
            }
            else
            {
                apiUrl = $"{apiUrl}?{param.Key}={param.Value}";
            }
        }

        return apiUrl;
    }

    public static (string, List<string>) GenerateTestValuesBasedOnType(KeyValue param)
    {
        if (int.TryParse(param.Value, out _))
        {
            var testValues = TestDataValues.IntegerValues;
            return ("int", testValues.Select(value => value.ToString()).ToList());
        }
        else if (long.TryParse(param.Value, out _))
        {
            var testValues = TestDataValues.LongValues;
            return ("long", testValues.Select(value => value.ToString()).ToList());
        }
        else if (double.TryParse(param.Value, out _))
        {
            var testValues = TestDataValues.DoubleValues;
            return ("double", testValues.Select(value => value.ToString()).ToList());
        }
        else if (float.TryParse(param.Value, out _))
        {
            var testValues = TestDataValues.FloatValues;
            return ("float", testValues.Select(value => value.ToString()).ToList());
        }
        else if (decimal.TryParse(param.Value, out _))
        {
            var testValues = TestDataValues.DecimalValues;
            return ("decimal", testValues.Select(value => value.ToString()).ToList());
        }
        else if (bool.TryParse(param.Value, out _))
        {
            var testValues = TestDataValues.BoolValues;
            return ("bool", testValues.Select(value => value.ToString()).ToList());
        }
        else if (DateTime.TryParse(param.Value, out _))
        {
            var testValues = TestDataValues.DateTimeValues;
            return ("DateTime", testValues.Select(value => value.ToString()).ToList());
        }
        else if (Guid.TryParse(param.Value, out _))
        {
            var testValues = TestDataValues.GuidValues;
            return ("Guid", testValues.Select(value => value.ToString()).ToList());
        }
        else if (char.TryParse(param.Value, out _))
        {
            var testValues = TestDataValues.CharValues;
            return ("char", testValues.Select(value => value.ToString()).ToList());
        }
        else
        {
            var testValues = TestDataValues.StringValues;
            return ("string", testValues.Select(value => value?.ToString()).ToList());
        }
    }
    public static List<GetDelTestInfo> GenerateTestObjectsBasedOnParams(GetDeleteTestingModel testingModel)
    {
        List<GetDelTestInfo> generatedTestObjects = new List<GetDelTestInfo>();

        var userProviderParamsUrl = GenerateURLFromParams(testingModel.APIUrl, null, null, testingModel.Params, -1);

        var userProviderCase = new GetDelTestInfo { URL = userProviderParamsUrl };
        generatedTestObjects.Add(userProviderCase);

        for (int index = 0; index < testingModel.Params.Count; index++)
        {
            var param = testingModel.Params[index];
            (string type, List<string> testValues) = GenerateTestValuesBasedOnType(param);
            foreach (var value in testValues)
            {
                var url = GenerateURLFromParams(testingModel.APIUrl, param.Key, value, testingModel.Params, index);
                var testObject = new GetDelTestInfo
                {
                    TestPropertyName = param.Key,
                    TestPropertyType = type,
                    TestPropertyValue = value,
                    URL = url
                };
                generatedTestObjects.Add(testObject);
            }
        }

        return generatedTestObjects;
    }
    public static List<GetDelTestInfo> GenerateTestObjectsBasedOnCustomParams(CustomGetDelTestModel testingModel)
    {
        List<GetDelTestInfo> generatedTestObjects = new List<GetDelTestInfo>();
        List<KeyValue> keyValuePairs = new List<KeyValue>();
        foreach (var param in testingModel.Params)
        {
            KeyValue kv = new KeyValue() { Key = param.Key, Value = param.Values.FirstOrDefault() };
            keyValuePairs.Add(kv);
        }

        for (int index = 0; index < testingModel.Params.Count; index++)
        {
            var param = testingModel.Params[index];

            foreach (var value in param.Values)
            {
                var url = GenerateURLFromParams(testingModel.APIUrl, param.Key, value, keyValuePairs, index);
                var testObject = new GetDelTestInfo
                {
                    TestPropertyName = param.Key,
                    TestPropertyValue = value,
                    URL = url
                };
                generatedTestObjects.Add(testObject);
            }
        }

        return generatedTestObjects;
    }

    public static async Task<List<TestPayloadInfo>> GenerateTestObjectsBasedOnJsonSchema(PostPutAPITestingModel testingModel)
    {
        try
        {
            var converter = new ExpandoObjectConverter();
            dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(testingModel.JsonSchema, converter);

            List<TestPayloadInfo> listOfTestPayloadInfo = await GenerateTestObjectHelper.CreateTestObjectsFromExpando(jsonObject);

            return listOfTestPayloadInfo;
        }
        catch (Exception ex)
        {

            throw new Exception($"Exception occurred when Generating Test Object : {ex.Message}");
        }
    }
}