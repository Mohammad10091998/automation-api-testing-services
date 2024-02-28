using APITestingService.Interface;
using Microsoft.Extensions.Logging;
using ModelsLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;

namespace APITestingService.Implementation
{
    public class APITestingServices : IAPITestingServices
    {
        private IHttpApiService _httpApiService;
        private ILogger<APITestingServices> _logger;
        public APITestingServices(IHttpApiService httpApiService, ILogger<APITestingServices> logger)
        {
            _httpApiService = httpApiService;
            _logger = logger;
        }
        public async Task<APITestingResponse> TestPostPutAPI(PostPutAPITestingModel testingModel)
        {
            try
            {
                _logger.LogInformation("APITestingServices.TestAPI - Started Generating TestObjects.");
                var generateTestObjects = await GenerateTestObjectHelper.GenerateTestObjectsBasedOnJsonSchema(testingModel);

                List<TestobjectInfo> testObjects = new List<TestobjectInfo>();
                int successCount = 0;
                int count = 1;
                var totalTestObjects = generateTestObjects.Count();
                foreach (var testObject in generateTestObjects)
                {
                    _logger.LogInformation($"APITestingServices.TestAPI - Looping : Test object number : {count}  Out Of : {totalTestObjects}");
                    var response = await _httpApiService.TestPostPutApiWithHttpClient(testObject, testingModel);
                    testObjects.Add(response);
                    successCount = response.IsSuccess ? successCount + 1 : successCount;
                    count++;
                }

                APITestingResponse apiTestingResponse = new APITestingResponse()
                {
                    TestedObjectInfos = testObjects,
                    FailureCalls = testObjects.Count - successCount,
                    SuccessCalls = successCount,
                    TotalTestedObjects = totalTestObjects
                };

                _logger.LogInformation("APITestingServices.TestAPI - Completed");
                return apiTestingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("APITestingServices.TestAPI - Failed");
                throw new Exception(ex.Message);
            }
        }

        public async Task<APITestingResponse> CustomTestPostPutAPI(CustomPostPutTestingModel testingModel)
        {
            try
            {
                _logger.LogInformation($"APITestingServices.CustomTestPostPutAPI - Started");
                List<TestobjectInfo> testObjectsResponse = new List<TestobjectInfo>();
                int successCount = 0;
                int count = 1;
                var totalTestObjects = testingModel.JsonSchemas.Count();
                foreach (var testObject in testingModel.JsonSchemas)
                {
                    _logger.LogInformation($"APITestingServices.CustomTestPostPutAPI - Looping : Test object number : {count}  Out Of : {totalTestObjects}");
                    var response = await _httpApiService.CustomTestPostPutApiWithHttpClient(testObject, testingModel);
                    testObjectsResponse.Add(response);
                    successCount = response.IsSuccess ? successCount + 1 : successCount;
                    count++;
                }

                APITestingResponse apiTestingResponse = new APITestingResponse()
                {
                    TestedObjectInfos = testObjectsResponse,
                    FailureCalls = testObjectsResponse.Count - successCount,
                    SuccessCalls = successCount,
                    TotalTestedObjects = totalTestObjects
                };

                _logger.LogInformation("APITestingServices.CustomTestPostPutAPI - Completed");
                return apiTestingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("APITestingServices.CustomTestPostPutAPI - Failed");
                throw new Exception(ex.Message);
            }
        }

        public async Task<APITestingResponse> TestGetDelAPI(GetDeleteTestingModel testingModel)
        {
            try
            {
                _logger.LogInformation($"APITestingServices.TestGetDelAPI - Started");

                List<GetDelTestInfo> generatedTestObjects = GenerateTestObjectHelper.GenerateTestObjectsBasedOnParams(testingModel);

                List<TestobjectInfo> testObjectsResponse = new List<TestobjectInfo>();
                int successCount = 0;
                int count = 1;
                var totalTestObjects = generatedTestObjects.Count();
                foreach (var testObject in generatedTestObjects)
                {
                    _logger.LogInformation($"APITestingServices.TestGetDelAPI - Looping : Test object number : {count}  Out Of : {totalTestObjects}");
                    var response = await _httpApiService.TestGetDelApiWithHttpClient(testObject, testingModel.MethodType, testingModel.Headers, testingModel.APIUrl);
                    testObjectsResponse.Add(response);
                    successCount = response.IsSuccess ? successCount + 1 : successCount;
                    count++;
                }

                APITestingResponse apiTestingResponse = new APITestingResponse()
                {
                    TestedObjectInfos = testObjectsResponse,
                    FailureCalls = testObjectsResponse.Count - successCount,
                    SuccessCalls = successCount,
                    TotalTestedObjects = totalTestObjects
                };

                _logger.LogInformation("APITestingServices.TestGetDelAPI - Completed");
                return apiTestingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("APITestingServices.TestGetDelAPI - Failed");
                throw new Exception(ex.Message);
            }
        }

        public async Task<APITestingResponse> CustomTestGetDelAPI(CustomGetDelTestModel testingModel)
        {
            try
            {
                _logger.LogInformation($"APITestingServices.CustomTestGetDelAPI - Started");

                List<GetDelTestInfo> generatedTestObjects = GenerateTestObjectHelper.GenerateTestObjectsBasedOnCustomParams(testingModel);

                List<TestobjectInfo> testObjectsResponse = new List<TestobjectInfo>();
                int successCount = 0;
                int count = 1;
                var totalTestObjects = generatedTestObjects.Count();
                foreach (var testObject in generatedTestObjects)
                {
                    _logger.LogInformation($"APITestingServices.CustomTestGetDelAPI - Looping : Test object number : {count}  Out Of : {totalTestObjects}");
                    var response = await _httpApiService.TestGetDelApiWithHttpClient(testObject, testingModel.MethodType, testingModel.Headers, testingModel.APIUrl);
                    testObjectsResponse.Add(response);
                    successCount = response.IsSuccess ? successCount + 1 : successCount;
                    count++;
                }

                APITestingResponse apiTestingResponse = new APITestingResponse()
                {
                    TestedObjectInfos = testObjectsResponse,
                    FailureCalls = testObjectsResponse.Count - successCount,
                    SuccessCalls = successCount,
                    TotalTestedObjects = totalTestObjects
                };

                _logger.LogInformation("APITestingServices.CustomTestGetDelAPI - Completed");
                return apiTestingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("APITestingServices.CustomTestGetDelAPI - Failed");
                throw new Exception(ex.Message); 
            }
        }

    }
}
