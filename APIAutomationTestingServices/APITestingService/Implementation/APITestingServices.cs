using APITestingService.Interface;
using Microsoft.Extensions.Logging;
using ModelsLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
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
        public async Task<APITestingResponse> TestPostPutAPI(APITestingModel testingModel)
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

        public async Task<APITestingResponse> TestGetDelAPI(GetDeleteTestingModel testingModel)
        {
            _logger.LogInformation($"APITestingServices.TestGetDelAPI - Started");

            List<GetDelTestInfo> generatedTestObjects = GenerateTestObjectsBasedOnParams(testingModel);

            List<TestobjectInfo> testObjectsResponse = new List<TestobjectInfo>();
            int successCount = 0;
            int count = 1;
            foreach (var testObject in generatedTestObjects)
            {
                _logger.LogInformation($"APITestingServices.TestGetDelAPI - Looping : Test object number : {count}");
                var response = await _httpApiService.TestGetDelApiWithHttpClient(testObject, testingModel);
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
                        NegativePropertyName = param.Key,
                        NegativePropertyType = type,
                        NegativePropertyValue = value,
                        URL = url
                    };
                    generatedTestObjects.Add(testObject);
                }
            }

            return generatedTestObjects;
        }

        private async Task<List<TestPayloadInfo>> GenerateTestObjectsBasedOnJsonSchema(APITestingModel testingModel)
        {
            var converter = new ExpandoObjectConverter();
            dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(testingModel.JsonSchema, converter);

            List<TestPayloadInfo> listOfTestPayloadInfo = await GenerateTestObjectHelper.CreateTestObjectsFromExpando(jsonObject);

            return listOfTestPayloadInfo;
        }
    }
}
