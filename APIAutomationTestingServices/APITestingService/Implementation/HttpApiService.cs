using APITestingService.Interface;
using Microsoft.Extensions.Logging;
using ModelsLibrary;
using System.Text;
using System.Text.Json;
//using System.Net.Http;

namespace APITestingService.Implementation
{
    public class HttpApiService : IHttpApiService
    {
        private HttpClient _client;
        private ILogger<HttpApiService> _logger;
        public HttpApiService(IHttpClientFactory httpClientFactory, ILogger<HttpApiService> logger)
        {
            _client = httpClientFactory.CreateClient();
            _logger = logger;
        }
        public async Task<TestobjectInfo> TestApiWithHttpClient(TestPayloadInfo info, APITestingModel model)
        {
            try
            {
                HttpRequestMessage httpRequestMessage = HttpRequestMessageBasedOnMethodType(model);
                _logger.LogInformation("HttpApiService.TestApiWithHttpClient - Making API call.");
                if (model.JsonSchema != null)
                {
                    var json = JsonSerializer.Serialize(info.TestObject);
                    var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpRequestMessage.Content = requestContent;
                }

                var response = await _client.SendAsync(httpRequestMessage);

                TestobjectInfo responseInfo = new TestobjectInfo
                {
                    TestedObject = info.TestObject,
                    NegativePropertyName = info.NegativePropertyName,
                    NegativePropertyType = info.NegativePropertyType,
                    NegativePropertyValue = info.NegativePropertyValue,
                    APIResponse = await response.Content.ReadAsStringAsync(),
                    StatusCode = response.StatusCode,
                    IsSuccess = response.IsSuccessStatusCode
                };
                _logger.LogInformation("HttpApiService.TestApiWithHttpClient - API call completed.");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogInformation("HttpApiService.TestApiWithHttpClient - API call failed.");
                throw new InvalidOperationException($"Exception Occurred while making api call : {e.Message}");
            }
        }
        private HttpRequestMessage HttpRequestMessageBasedOnMethodType(APITestingModel testingModel)
        {
            HttpRequestMessage httpRequestMessage = null;
            if (testingModel.MethodType == "Get")
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, testingModel.APIUrl);
            }
            else if (testingModel.MethodType == "Post")
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, testingModel.APIUrl);
            }
            else if (testingModel.MethodType == "Put")
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, testingModel.APIUrl);
            }
            else if (testingModel.MethodType == "Delete")
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, testingModel.APIUrl);
            }
            else
            {
                throw new InvalidDataException("Invalid Method Type");
            }

            if (testingModel.HeaderKeyValues != null && testingModel.HeaderKeyValues.Count() > 0)
            {
                foreach (var header in testingModel.HeaderKeyValues)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }
            return httpRequestMessage;
        }
    }
}
