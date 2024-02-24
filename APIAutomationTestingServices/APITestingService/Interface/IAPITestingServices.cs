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
        Task<APITestingResponse> TestPostPutAPI(PostPutAPITestingModel testingModel);
        /// <summary>
        /// Test custom test objects provided by user
        /// </summary>
        /// <param name="testingModel"></param>
        /// <returns></returns>
        Task<APITestingResponse> CustomTestPostPutAPI(CustomPostPutTestingModel testingModel);
        /// <summary>
        /// Generate Params values bases on given query parameter
        /// </summary>
        /// <param name="testingModel"></param>
        /// <returns></returns>
        Task<APITestingResponse> TestGetDelAPI(GetDeleteTestingModel testingModel);
        /// <summary>
        /// Test custom parameter provided by user
        /// </summary>
        /// <param name="testingModel"></param>
        /// <returns></returns>
        Task<APITestingResponse> CustomTestGetDelAPI(CustomGetDelTestModel testingModel);
    }
}
