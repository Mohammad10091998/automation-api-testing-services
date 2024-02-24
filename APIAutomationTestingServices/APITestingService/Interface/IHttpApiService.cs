using ModelsLibrary;
using System.Reflection;

namespace APITestingService.Interface
{
    public interface IHttpApiService
    {
        /// <summary>
        /// Test Post and Put API for given info
        /// </summary>
        /// <param name="info">Contains payload and negative property info</param>
        /// <param name="model">Contains info url, headers, method type, json schema</param>
        /// <param name="requestMessage">Contains info request message</param>
        /// <returns>Returns the TestobjectInfo which contains api response, status code etc </returns>
        Task<TestobjectInfo> TestPostPutApiWithHttpClient(TestPayloadInfo info, PostPutAPITestingModel model);
        /// <summary>
        /// Test Post and Put API for given info
        /// </summary>
        /// <param name="info">Contains payload and negative property info</param>
        /// <param name="model">Contains info url, headers, method type, json schema</param>
        /// <param name="requestMessage">Contains info request message</param>
        /// <returns>Returns the TestobjectInfo which contains api response, status code etc </returns>
        Task<TestobjectInfo> CustomTestPostPutApiWithHttpClient(KeyValue info, CustomPostPutTestingModel model);
        /// <summary>
        /// Test Get and Delete API
        /// </summary>
        /// <param name="info"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<TestobjectInfo> TestGetDelApiWithHttpClient(GetDelTestInfo info, string methodType, List<KeyValue> headers, string url);

    }
}
