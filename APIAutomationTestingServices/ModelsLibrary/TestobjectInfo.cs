using System.Net;

namespace ModelsLibrary
{
    public class TestobjectInfo
    {
        public dynamic TestedObject { get; set; }
        public string? TestPropertyName { get; set; }
        public string? TestPropertyValue { get; set; }
        public string? TestPropertyType { get; set; }
        public string APIResponse { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
    }
}
