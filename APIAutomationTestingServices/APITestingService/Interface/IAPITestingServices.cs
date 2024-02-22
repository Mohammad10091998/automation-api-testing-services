using ModelsLibrary;

namespace APITestingService.Interface
{
    public interface IAPITestingServices
    {
        /// <summary>
        /// Generate testobjects based on Json schema string 
        /// </summary>
        /// <param name="testingModel"></param>
        /// <returns>A task that represents the async operation</returns>
        Task<APITestingResponse> TestPostPutAPI(APITestingModel testingModel);
        Task<APITestingResponse> TestGetDelAPI(GetDeleteTestingModel testingModel);
    }
}
