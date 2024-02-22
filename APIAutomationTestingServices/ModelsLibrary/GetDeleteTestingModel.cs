namespace ModelsLibrary
{
    public class GetDeleteTestingModel
    {
        public string APIUrl { get; set; }
        public string MethodType { get; set; }
        public List<KeyValue> Headers { get; set; }
        public List<KeyValue> Params { get; set; }
        public GetDeleteTestingModel()
        {
            Headers = new List<KeyValue>();
            Params = new List<KeyValue>();
        }
    }
}
