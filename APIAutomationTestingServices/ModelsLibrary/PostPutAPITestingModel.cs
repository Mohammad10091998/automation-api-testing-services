namespace ModelsLibrary
{
    public class PostPutAPITestingModel
    {
        public string APIUrl { get; set; }
        public string MethodType { get; set; }
        public string JsonSchema { get; set; }
        public List<KeyValue> Headers { get; set; }
        public PostPutAPITestingModel()
        {
            Headers = new List<KeyValue>();
        }
    }
}
