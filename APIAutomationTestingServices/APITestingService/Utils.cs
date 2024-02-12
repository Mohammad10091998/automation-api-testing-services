using System.Collections;
using System.Dynamic;
using System.Linq.Dynamic.Core;
using ModelsLibrary;

public static class Utils
{
    public static int count;
    public static int currentCount;
    public async static Task<List<dynamic>> GetTypedObject(dynamic input)
    {
        try
        {
            if (input is ExpandoObject)
            {
                LinkedList<Node> listOfProperties = new LinkedList<Node>();
                Type type = CreateAbstractClassType(input, listOfProperties);
                var obj = CreateObject(type, input);

                var testObjests = GenerateTestObjects(listOfProperties, type, input);
                return testObjests;
            }
            throw new Exception("Invalid Data");
        }
        catch (Exception)
        {

            throw new InvalidDataException("Invalid Data");
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

    public static object CreateObject(Type type, dynamic input)
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
                    val = CreateObject(propType, expando.Value);
                }
                else if (expando.Value is IList)
                {
                    var internalType = propInfo.PropertyType.GenericTypeArguments.FirstOrDefault() ?? typeof(object);
                    var temp = (IList)expando.Value;
                    var newList = new List<object>().Cast(internalType).ToList(internalType);
                    for (int i = 0; i < temp.Count; i++)
                    {
                        var child = CreateObject(internalType, temp[i]);
                        newList.Add(child);
                    };
                    val = newList;
                }
                else
                {
                    val = expando.Value;
                }
                propInfo.SetValue(obj, val, null);
            }
        }

        return obj;
    }

    public static object CreateDiffrentTestObject(Type type, dynamic input, string updatingKey, object updatingValue)
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
                    val = CreateDiffrentTestObject(propType, expando.Value, updatingKey, updatingValue);
                }
                else if (expando.Value is IList)
                {
                    var internalType = propInfo.PropertyType.GenericTypeArguments.FirstOrDefault() ?? typeof(object);
                    var temp = (IList)expando.Value;
                    var newList = new List<object>().Cast(internalType).ToList(internalType);
                    for (int i = 0; i < temp.Count; i++)
                    {
                        var child = CreateDiffrentTestObject(internalType, temp[i], updatingKey, updatingValue);
                        newList.Add(child);
                    };
                    val = newList;
                }
                else
                {
                    val = expando.Value;
                    if (expando.Key == updatingKey)
                    {
                        if (count == currentCount)
                        {
                            val = updatingValue;
                        }

                        currentCount++;

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
    public static List<dynamic> GenerateTestObjects(LinkedList<Node> listOfProperties, Type type, dynamic input)
    {
        List<dynamic> listOfTestObject = new List<dynamic>();

        List<int> ints = new List<int>() { -9, 9, 99 };

        Dictionary<string, int> keyCountMap = new Dictionary<string, int>();

        foreach (var item in listOfProperties)
        {
            if (keyCountMap.ContainsKey(item.Key))
            {
                count = keyCountMap[item.Key] + 1;
                keyCountMap[item.Key] = count;
            }
            else
            {
                keyCountMap.Add(item.Key, 0);
                count = 0;
            }

            if (item.PropertyType == "Int64")
            {
                foreach (var v in ints)
                {
                    currentCount = 0;
                    var testObject = CreateDiffrentTestObject(type, input, item.Key, v);
                    listOfTestObject.Add(testObject);
                }
            }
        }

        return listOfTestObject;
    }
}