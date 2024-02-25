using System.Collections;
using System.Dynamic;
using System.Linq.Dynamic.Core;
using System.Security;
using System.Text.Json.Nodes;
using ModelsLibrary;
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
                AddNodeInTheList(expando.Key, value, list);
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
    private static void AddNodeInTheList(string key, Type value, LinkedList<Node> list)
    {
        string typeName = value.Name;
        if (typeName == "Int64")
        {
            Node node = new Node(key, typeName);
            list.AddLast(node);
        }
        else if (typeName == "String")
        {
            Node node = new Node(key, typeName);
            list.AddLast(node);
        }
        else if (typeName == "DateTime")
        {
            Node node = new Node(key, typeName);
            list.AddLast(node);
        }
    }
    public static List<TestPayloadInfo> GenerateUniqueTestObjects(LinkedList<Node> propertyNodes, Type objectType, dynamic input)
    {
        List<TestPayloadInfo> uniqueTestObjects = new List<TestPayloadInfo>();

        List<int> integerValues = new List<int>() { int.MinValue, 0, int.MaxValue };
        List<string> stringValues = new List<string>() { null, "", "abc123", "!@#$%","abc_xyx","abc.xyz", "abc xyz" };

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

            if (propertyNode.PropertyType == "Int64")
            {
                foreach (var integerValue in integerValues)
                {                                                                                                   
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, integerValue, propertyIndex);
                    TestPayloadInfo testPayloadInfo = new TestPayloadInfo
                    {
                        TestObject = testObject,
                        NegativePropertyName = propertyNode.Key,
                        NegativePropertyValue = integerValue.ToString(),
                        NegativePropertyType = propertyNode.PropertyType
                    };
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
            if (propertyNode.PropertyType == "String")
            {
                foreach (var value in stringValues)
                {
                    currentIndex = 0;
                    var testObject = CreateDifferentTestObject(objectType, input, propertyNode.Key, value, propertyIndex);
                    TestPayloadInfo testPayloadInfo = new TestPayloadInfo
                    {
                        TestObject = testObject,
                        NegativePropertyName = propertyNode.Key,
                        NegativePropertyValue = value,
                        NegativePropertyType = propertyNode.PropertyType
                    };
                    uniqueTestObjects.Add(testPayloadInfo);
                }
            }
        }

        return uniqueTestObjects;
    }

}