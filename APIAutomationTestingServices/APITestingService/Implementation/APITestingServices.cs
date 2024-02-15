using APITestingService.Interface;
using Microsoft.Extensions.Logging;
using ModelsLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using System.Reflection;

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
        public async Task<APITestingResponse> TestAPI(APITestingModel testingModel)
        {
            _logger.LogInformation("APITestingServices.TestAPI - Started Generating TestObjects.");
            var generateTestObjects = await GenerateTestObjectsBasedOnJsonSchema(testingModel);

            List<TestobjectInfo> testObjects = new List<TestobjectInfo>();
            int successCount = 0;
            int count = 1;
            foreach(var testObject in generateTestObjects)
            {
                _logger.LogInformation($"APITestingServices.TestAPI - Looping : Test object number : {count}");
                var response = await _httpApiService.TestApiWithHttpClient(testObject, testingModel);
                testObjects.Add(response);
                successCount = response.IsSuccess ? successCount + 1 : successCount;
                count++;
            }

            APITestingResponse apiTestingResponse = new APITestingResponse()
            {
                TestedObjectInfos = testObjects,
                FailureCalls = testObjects.Count - successCount,
                SuccessCalls = successCount,
                TotalTestedObjects = testObjects.Count
            };

            _logger.LogInformation("APITestingServices.TestAPI - Completed");
            return apiTestingResponse;
        }
        private async Task<List<TestPayloadInfo>> GenerateTestObjectsBasedOnJsonSchema(APITestingModel testingModel)
        {
            var converter = new ExpandoObjectConverter();
            dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(testingModel.JsonSchema, converter);

            List<TestPayloadInfo> listOfTestPayloadInfo = await Utils.CreateTestObjectsFromExpando(jsonObject);

            //List<string> jsonResults = new List<string>();

            //foreach (var testObject in testObjests)
            //{
            //    string jsonResult = JsonConvert.SerializeObject(testObject, Formatting.Indented);
            //    jsonResults.Add(jsonResult);
            //}

            return listOfTestPayloadInfo;
        }
    }
}
