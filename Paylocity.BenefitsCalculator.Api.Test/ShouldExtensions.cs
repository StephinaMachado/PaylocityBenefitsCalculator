using Newtonsoft.Json;
using Paylocity.BenefitsCalculator.Common.Dtos;
using System.Net;
using Xunit;

namespace Api.UnitTest
{
    internal static class ShouldExtensions
    {
        public static Task ShouldReturn(this HttpResponseMessage response, HttpStatusCode statusCode)
        {
            AssertCommonResponseParts(response, statusCode);
            return Task.CompletedTask;
        }

        public static async Task ShouldReturn<T>(this HttpResponseMessage response, HttpStatusCode statusCode, T expectedContent)
        {
            await response.ShouldReturn(statusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(await response.Content.ReadAsStringAsync());
            Assert.True(apiResponse.Success);
            Assert.Equal(JsonConvert.SerializeObject(expectedContent), JsonConvert.SerializeObject(apiResponse.Data));
        }

        private static void AssertCommonResponseParts(this HttpResponseMessage response, HttpStatusCode statusCode)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
