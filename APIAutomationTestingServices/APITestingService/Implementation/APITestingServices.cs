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
            var converter = new ExpandoObjectConverter();
            dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(testingModel.JsonSchema, converter);

            List<dynamic> testObjests = await Utils.CreateTestObjectsFromExpando(jsonObject);

            //List<string> jsonResults = new List<string>();

            //foreach (var testObject in testObjests)
            //{
            //    string jsonResult = JsonConvert.SerializeObject(testObject, Formatting.Indented);
            //    jsonResults.Add(jsonResult);
            //}

            return testObjests;
        }
    }
}
