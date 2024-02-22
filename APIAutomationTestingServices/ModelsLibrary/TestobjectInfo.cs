using System.Net;

namespace ModelsLibrary
{
    public class TestobjectInfo
    {
        public dynamic TestedObject { get; set; }
        public string? NegativePropertyName { get; set; }
        public string? NegativePropertyValue { get; set; }
        public string? NegativePropertyType { get; set; }
        public string APIResponse { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
    }
}
