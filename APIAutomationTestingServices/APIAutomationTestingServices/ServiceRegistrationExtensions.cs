using APITestingService.Implementation;
using APITestingService.Interface;

namespace APIAutomationTestingServices
{
    public static class ServiceRegistrationExtensions
    {
        /// <summary>
        /// Register services to IOC container
        /// </summary>
        /// <param name="collection">Collection of service</param>
        public static void RegisterService(this IServiceCollection collection)
        {
            collection.AddScoped<IAPITestingServices, APITestingServices>();
            collection.AddScoped<IHttpApiService, HttpApiService>();
        }
    }
}
