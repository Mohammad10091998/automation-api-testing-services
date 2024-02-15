namespace ModelsLibrary
{
    public class APITestingModel
    {
        public string APIUrl { get; set; }
        public string MethodType { get; set; }
        public string JsonSchema { get; set; }
        public List<HeaderKeyValue> HeaderKeyValues { get; set; }
        public APITestingModel()
        {
            HeaderKeyValues= new List<HeaderKeyValue>();
        }
    }
}
