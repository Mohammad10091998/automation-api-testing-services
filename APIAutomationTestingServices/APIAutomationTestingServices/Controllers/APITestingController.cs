using APIAutomationTestingServices.Filter;
using APITestingService.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary;

namespace APIAutomationTestingServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APITestingController : ControllerBase
    {
        public IAPITestingServices _apiTestingServices;
        private readonly ILogger<APITestingController> _logger;
        public APITestingController(IAPITestingServices apiTestingServices, ILogger<APITestingController> logger)
        {
            _apiTestingServices = apiTestingServices;
            _logger = logger;
        }

        [HttpPost("TestPostPut")]
        [APITestingValidatorFilter(PropertyName = "testModel")]
        public async Task<ActionResult<APITestingResponse>> TestPostPutAPITestResults([FromBody] PostPutAPITestingModel testModel)
        {
            try
            {
                _logger.LogInformation("APITestingController.GetAPITestResults - Starting to get Test Results.");
                var response = await _apiTestingServices.TestPostPutAPI(testModel);
                _logger.LogInformation("APITestingController.GetAPITestResults - Completed.");
                return response;
            }
            catch (Exception e)
            {
                _logger.LogInformation("APITestingController.GetAPITestResults - Failed to get Test Results.");
                return BadRequest(e.Message);
            }
        }
        [HttpPost("CustomTestPostPut")]
        [APITestingValidatorFilter(PropertyName = "testModel")]
        public async Task<ActionResult<APITestingResponse>> CustomTestPostPutAPITestResults([FromBody] CustomPostPutTestingModel testModel)
        {
            try
            {
                _logger.LogInformation("APITestingController.CustomTestPostPutAPITestResults - Starting to get Test Results.");
                var response = await _apiTestingServices.CustomTestPostPutAPI(testModel);
                _logger.LogInformation("APITestingController.CustomTestPostPutAPITestResults - Completed.");
                return response;
            }
            catch (Exception e)
            {
                _logger.LogInformation("APITestingController.CustomTestPostPutAPITestResults - Failed to get Test Results.");
                return BadRequest(e.Message);
            }
        }
        [HttpPost("TestGetDel")]
        [APITestingValidatorFilter(PropertyName = "testModel")]
        public async Task<ActionResult<APITestingResponse>> TestGetDelAPITestResults([FromBody] GetDeleteTestingModel testModel)
        {
            try
            {
                _logger.LogInformation("APITestingController.TestGetDelAPITestResults - Starting to get Test Results.");
                var response = await _apiTestingServices.TestGetDelAPI(testModel);
                _logger.LogInformation("APITestingController.TestGetDelAPITestResults - Completed.");
                return response;
            }
            catch (Exception e)
            {
                _logger.LogInformation("APITestingController.TestGetDelAPITestResults - Failed to get Test Results.");
                return BadRequest(e.Message);
            }
        }
        [HttpPost("CustomTestGetDel")]
        [APITestingValidatorFilter(PropertyName = "testModel")]
        public async Task<ActionResult<APITestingResponse>> CustomTestGetDelAPITestResults([FromBody] CustomGetDelTestModel testModel)
        {
            try
            {
                _logger.LogInformation("APITestingController.CustomTestGetDelAPITestResults - Starting to get Test Results.");
                var response = await _apiTestingServices.CustomTestGetDelAPI(testModel);
                _logger.LogInformation("APITestingController.CustomTestGetDelAPITestResults - Completed.");
                return response;
            }
            catch (Exception e)
            {
                _logger.LogInformation("APITestingController.CustomTestGetDelAPITestResults - Failed to get Test Results.");
                return BadRequest(e.Message);
            }
        }
    }
}
