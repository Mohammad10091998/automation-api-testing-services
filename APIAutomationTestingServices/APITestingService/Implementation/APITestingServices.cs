using APITestingService.Interface;
using ModelsLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;

namespace APITestingService.Implementation
{
    public class APITestingServices : IAPITestingServices
    {
        public async Task<APITestingResponse> TestAPI(APITestingModel testingModel)
        {
            var generateTestObjects = GenerateTestObjectsBasedOnJsonSchema(testingModel);
            return null;
        }
        private async Task<List<dynamic>> GenerateTestObjectsBasedOnJsonSchema(APITestingModel testingModel)
        {
            //string jsonString = "{\"Id\":1,\"Level1Name\":\"Test\",\"DOB\":\"2024-02-09T05:19:04.5606721Z\",\"Level2\":{\"Id\":10,\"Name\":\"Name1\"},\"Level2List\":[{\"Id\":100,\"Comment\":\"Comment1\",\"CommentNumber\":1}]}";
            
            var converter = new ExpandoObjectConverter();

            dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(testingModel.JsonSchema, converter);

            List<dynamic> testObjests = await Utils.GetTypedObject(jsonObject);
          
            return testObjests;
        }
    }
}
