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
                var generateTestObjects = await GenerateTestObjectsBasedOnJsonSchema(testingModel);

                List<TestobjectInfo> testObjects = new List<TestobjectInfo>();
                int successCount = 0;
                int count = 1;
                foreach (var testObject in generateTestObjects)
                {
                    _logger.LogInformation($"APITestingServices.TestAPI - Looping : Test object number : {count}");
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
                    TotalTestedObjects = testObjects.Count
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
                foreach (var testObject in testingModel.JsonSchemas)
                {
                    _logger.LogInformation($"APITestingServices.CustomTestPostPutAPI - Looping : Test object number : {count}");
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
                    TotalTestedObjects = testObjectsResponse.Count
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

                List<GetDelTestInfo> generatedTestObjects = GenerateTestObjectsBasedOnParams(testingModel);

                List<TestobjectInfo> testObjectsResponse = new List<TestobjectInfo>();
                int successCount = 0;
                int count = 1;
                foreach (var testObject in generatedTestObjects)
                {
                    _logger.LogInformation($"APITestingServices.TestGetDelAPI - Looping : Test object number : {count}");
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
                    TotalTestedObjects = testObjectsResponse.Count
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

                List<GetDelTestInfo> generatedTestObjects = GenerateTestObjectsBasedOnCustomParams(testingModel);

                List<TestobjectInfo> testObjectsResponse = new List<TestobjectInfo>();
                int successCount = 0;
                int count = 1;
                foreach (var testObject in generatedTestObjects)
                {
                    _logger.LogInformation($"APITestingServices.CustomTestGetDelAPI - Looping : Test object number : {count}");
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
                    TotalTestedObjects = testObjectsResponse.Count
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

        private string GenerateURLFromParams(string apiUrl, string key, string value, List<KeyValue> @params, int index)
        {
            if (key != null && value != null)
            {
                apiUrl = $"{apiUrl}?{key}={value}";
            }
            for (int ind = 0; ind < @params.Count; ind++)
            {
                if (ind == index) continue;
                var param = @params[ind];
                if (apiUrl.Contains("?"))
                {
                    apiUrl = $"{apiUrl}&{param.Key}={param.Value}";
                }
                else
                {
                    apiUrl = $"{apiUrl}?{param.Key}={param.Value}";
                }
            }

            return apiUrl;
        }

        private (string, List<string>) GenerateTestValuesBasedOnType(KeyValue param)
        {
            if (int.TryParse(param.Value, out _))
            {
                List<int> testValues = new List<int>() { -1, 0, 1 };
                return ("int", testValues.Select(value => value.ToString()).ToList());
            }
            else if (long.TryParse(param.Value, out _))
            {
                List<long> testValues = new List<long>() { -1, 0, 1 };
                return ("long", testValues.Select(value => value.ToString()).ToList());
            }
            else if (double.TryParse(param.Value, out _))
            {
                List<double> testValues = new List<double>() { -1.0, 0.0, 1.0 };
                return ("double", testValues.Select(value => value.ToString()).ToList());
            }
            else if (bool.TryParse(param.Value, out _))
            {
                List<bool> testValues = new List<bool>() { true, false };
                return ("bool", testValues.Select(value => value.ToString()).ToList());
            }
            else if (DateTime.TryParse(param.Value, out _))
            {
                DateTime currentDate = DateTime.Now;
                List<DateTime> testValues = new List<DateTime>()
                {
                    currentDate.AddYears(-100),
                    currentDate,
                    currentDate.AddYears(100)
                };
                return ("", testValues.Select(value => value.ToString()).ToList());
            }
            else if (Guid.TryParse(param.Value, out _))
            {
                List<Guid> testValues = new List<Guid>() { Guid.Empty };
                return ("Guid", testValues.Select(value => value.ToString()).ToList());
            }
            else
            {
                List<string> testValues = new List<string>() { "", "xyx" };
                return ("string", testValues.Select(value => value.ToString()).ToList());
            }
        }
        private List<GetDelTestInfo> GenerateTestObjectsBasedOnParams(GetDeleteTestingModel testingModel)
        {
            List<GetDelTestInfo> generatedTestObjects = new List<GetDelTestInfo>();

            var userProviderParamsUrl = GenerateURLFromParams(testingModel.APIUrl, null, null, testingModel.Params, -1);

            var userProviderCase = new GetDelTestInfo { URL = userProviderParamsUrl };
            generatedTestObjects.Add(userProviderCase);

            for (int index = 0; index < testingModel.Params.Count; index++)
            {
                var param = testingModel.Params[index];
                (string type, List<string> testValues) = GenerateTestValuesBasedOnType(param);
                foreach (var value in testValues)
                {
                    var url = GenerateURLFromParams(testingModel.APIUrl, param.Key, value, testingModel.Params, index);
                    var testObject = new GetDelTestInfo
                    {
                        TestPropertyName = param.Key,
                        TestPropertyType = type,
                        TestPropertyValue = value,
                        URL = url
                    };
                    generatedTestObjects.Add(testObject);
                }
            }

            return generatedTestObjects;
        }
        private List<GetDelTestInfo> GenerateTestObjectsBasedOnCustomParams(CustomGetDelTestModel testingModel)
        {
            List<GetDelTestInfo> generatedTestObjects = new List<GetDelTestInfo>();
            List<KeyValue> keyValuePairs = new List<KeyValue>();
            foreach (var param in testingModel.Params)
            {
                KeyValue kv = new KeyValue() { Key = param.Key, Value = param.Values.FirstOrDefault() };
                keyValuePairs.Add(kv);
            }

            for (int index = 0; index < testingModel.Params.Count; index++)
            {
                var param = testingModel.Params[index];
                
                foreach (var value in param.Values)
                {
                    var url = GenerateURLFromParams(testingModel.APIUrl, param.Key, value, keyValuePairs, index);
                    var testObject = new GetDelTestInfo
                    {
                        TestPropertyName = param.Key,
                        TestPropertyValue = value,
                        URL = url
                    };
                    generatedTestObjects.Add(testObject);
                }
            }

            return generatedTestObjects;
        }

        private async Task<List<TestPayloadInfo>> GenerateTestObjectsBasedOnJsonSchema(PostPutAPITestingModel testingModel)
        {
            try
            {
                var converter = new ExpandoObjectConverter();
                dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(testingModel.JsonSchema, converter);

                List<TestPayloadInfo> listOfTestPayloadInfo = await GenerateTestObjectHelper.CreateTestObjectsFromExpando(jsonObject);

                return listOfTestPayloadInfo;
            }
            catch (Exception ex)
            {

                throw new Exception($"Exception occurred when Generating Test Object : {ex.Message}");
            }
        }
    }
}
