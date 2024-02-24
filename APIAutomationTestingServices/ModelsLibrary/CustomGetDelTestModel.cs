namespace ModelsLibrary
{
    public class CustomGetDelTestModel
    {
        public string APIUrl { get; set; }
        public string MethodType { get; set; }
        public List<KeyValue> Headers { get; set; }
        public List<ParamKeyValues> Params { get; set; }
        public CustomGetDelTestModel()
        {
            Headers = new List<KeyValue>();
            Params = new List<ParamKeyValues>();
        }
    }
}
