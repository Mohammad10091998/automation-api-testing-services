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

        [HttpPost]
        public async Task<ActionResult<APITestingResponse>> GetAPITestResults(APITestingModel testModel)
        {
            try
            {
                _logger.LogInformation("APITestingController.GetAPITestResults - Starting to get Test Results.");
                var response = await _apiTestingServices.TestAPI(testModel);
                _logger.LogInformation("APITestingController.GetAPITestResults - Completed.");
                return response;
            }
            catch (Exception e)
            {
                _logger.LogInformation("APITestingController.GetAPITestResults - Failed to get Test Results.");
                return BadRequest(e.Message);
            }
        }
    }
}
