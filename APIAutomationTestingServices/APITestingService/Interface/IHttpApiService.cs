using ModelsLibrary;

namespace APITestingService.Interface
{
    public interface IHttpApiService
    {
        /// <summary>
        /// Test an API for given info
        /// </summary>
        /// <param name="info">Contains payload and negative property info</param>
        /// <param name="model">Contains info url, headers, method type, json schema</param>
        /// <param name="requestMessage">Contains info request message</param>
        /// <returns>Returns the TestobjectInfo which contains api response, status code etc </returns>
        Task<TestobjectInfo> TestApiWithHttpClient(TestPayloadInfo info, APITestingModel model);

    }
}
