namespace ModelsLibrary
{
    public class CustomPostPutTestingModel
    {
        public string APIUrl { get; set; }
        public string MethodType { get; set; }
        public List<KeyValue> JsonSchemas { get; set; }
        public List<KeyValue> Headers { get; set; }
        public CustomPostPutTestingModel()
        {
            Headers = new List<KeyValue>();
            JsonSchemas = new List<KeyValue>();
        }
    }
}
