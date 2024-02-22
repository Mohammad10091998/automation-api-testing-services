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
        public async Task<ActionResult<APITestingResponse>> TestPostPutAPITestResults(APITestingModel testModel)
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
        [HttpPost("TestGetDel")]
        public async Task<ActionResult<APITestingResponse>> TestGetDelAPITestResults(GetDeleteTestingModel testModel)
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
    }
}
