using Paylocity.BenefitsCalculator.Common.Models;
using System.Text;
using System.Text.Json;

namespace Api.UnitTest
{
    public class IntegrationTest : IDisposable
    {
        private HttpClient? _httpClient;

        protected HttpClient HttpClient
        {
            get
            {
                if (_httpClient == default)
                {
                    _httpClient = new HttpClient
                    {
                        BaseAddress = new Uri("http://localhost:5273")
                    };
                    _httpClient.DefaultRequestHeaders.Add("Accept", "text/plain");
                }
                return _httpClient;
            }
        }

        public async Task<HttpResponseMessage> CreateEmployee(CreateEmployeeModel employee)
        {
            var serializedObject = JsonSerializer.Serialize(employee);
            var data = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            return await HttpClient.PostAsync("/api/v1/employee", data);
        }

        public async Task<HttpResponseMessage> CreateDependent(CreateDependentModel dependent)
        {
            var serializedObject = JsonSerializer.Serialize(dependent);
            var data = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            return await HttpClient.PostAsync("/api/v1/EmployeeDependent", data);
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }
    }
}
