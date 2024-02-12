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
        Task<APITestingResponse> TestAPI(APITestingModel testingModel);
    }
}
