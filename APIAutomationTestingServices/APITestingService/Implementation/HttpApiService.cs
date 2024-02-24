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
        public async Task<TestobjectInfo> TestPostPutApiWithHttpClient(TestPayloadInfo info, PostPutAPITestingModel model)
        {
            try
            {
                HttpRequestMessage httpRequestMessage = HttpRequestMessageBasedOnMethodType(model.MethodType,model.Headers,model.APIUrl);
                _logger.LogInformation("HttpApiService.TestApiWithHttpClient - Making API call.");
                if (info.TestObject != null)
                {
                    var json = JsonSerializer.Serialize(info.TestObject);
                    var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpRequestMessage.Content = requestContent;
                }

                var response = await _client.SendAsync(httpRequestMessage);

                TestobjectInfo responseInfo = new TestobjectInfo
                {
                    TestedObject = info.TestObject,
                    TestPropertyName = info.NegativePropertyName,
                    TestPropertyType = info.NegativePropertyType,
                    TestPropertyValue = info.NegativePropertyValue,
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

        public async Task<TestobjectInfo> CustomTestPostPutApiWithHttpClient(KeyValue info, CustomPostPutTestingModel model)
        {
            try
            {
                HttpRequestMessage httpRequestMessage = HttpRequestMessageBasedOnMethodType(model.MethodType, model.Headers, model.APIUrl);
                _logger.LogInformation("HttpApiService.TestApiWithHttpClient - Making API call.");
                if (info.Value != null)
                {
                    var requestContent = new StringContent(info.Value, Encoding.UTF8, "application/json");
                    httpRequestMessage.Content = requestContent;
                }

                var response = await _client.SendAsync(httpRequestMessage);

                TestobjectInfo responseInfo = new TestobjectInfo
                {
                    TestedObject = info.Value,
                    TestPropertyName = info.Key,
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

        public async Task<TestobjectInfo> TestGetDelApiWithHttpClient(GetDelTestInfo info, string methodType, List<KeyValue> headers, string url)
        {
            try
            {
                HttpRequestMessage httpRequestMessage = HttpRequestMessageBasedOnMethodType(methodType, headers, url);
                _logger.LogInformation("HttpApiService.TestGetDelApiWithHttpClient - Making API call.");
               
                var response = await _client.SendAsync(httpRequestMessage);

                TestobjectInfo responseInfo = new TestobjectInfo
                {
                    TestedObject = info.URL,
                    TestPropertyName = info.TestPropertyName,
                    TestPropertyType = info.TestPropertyType,
                    TestPropertyValue = info.TestPropertyValue,
                    APIResponse = await response.Content.ReadAsStringAsync(),
                    StatusCode = response.StatusCode,
                    IsSuccess = response.IsSuccessStatusCode
                };
                _logger.LogInformation("HttpApiService.TestGetDelApiWithHttpClient - API call completed.");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogInformation("HttpApiService.TestGetDelApiWithHttpClient - API call failed.");
                throw new InvalidOperationException($"Exception Occurred while making api call : {e.Message}");
            }
        }
        private HttpRequestMessage HttpRequestMessageBasedOnMethodType(string methodType, List<KeyValue> headers, string url)
        {
            HttpRequestMessage httpRequestMessage = null;
            if (methodType == "Get")
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            }
            else if (methodType == "Post")
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            }
            else if (methodType == "Put")
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, url);
            }
            else if (methodType == "Delete")
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, url);
            }
            else
            {
                throw new InvalidDataException("Invalid Method Type");
            }

            if (headers != null && headers.Count() > 0)
            {
                foreach (var header in headers)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }
            return httpRequestMessage;
        }
    }
}
