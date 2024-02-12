namespace ModelsLibrary
{
    public class Node
    {
        public string Key { get; set; }
        public string PropertyType { get; set; }
        public Node(string key, string propertyType)
        {
            Key = key;
            PropertyType = propertyType;
        }
    }
}
