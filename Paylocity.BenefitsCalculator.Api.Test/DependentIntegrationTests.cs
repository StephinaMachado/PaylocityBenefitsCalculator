using Newtonsoft.Json;
using Paylocity.BenefitsCalculator.Common.Dtos;
using Paylocity.BenefitsCalculator.Common.Dtos.Dependent;
using Paylocity.BenefitsCalculator.Common.Enums;
using Paylocity.BenefitsCalculator.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Api.UnitTest
{
    public class DependentIntegrationTests : IntegrationTest
    {

        [Fact]
        //task: make test pass
        public async Task WhenAskedForAllDependents_ShouldReturnAllDependents()
        {
            var response = await CreateEmployee(new CreateEmployeeModel { FirstName = "SpouseMorant", LastName = "LastMorant", DateOfBirth = new DateTime(1990, 5, 5), Salary = 100000 });
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<int?>>(await response.Content.ReadAsStringAsync());
            var response1 = await CreateEmployee(new CreateEmployeeModel { FirstName = "SpouseMorant1", LastName = "LastMorant1", DateOfBirth = new DateTime(1992, 5, 5), Salary = 100000 });
            var apiResponse1 = JsonConvert.DeserializeObject<ApiResponse<int?>>(await response1.Content.ReadAsStringAsync());

            var dependents = new List<CreateDependentModel>
        {
            new()
            {
                Id = 1,
                FirstName = "Spouse",
                LastName = "Morant",
                 EmployeeId = apiResponse.Data,
                Relationship = Relationship.Spouse,
                DateOfBirth = new DateTime(1998, 3, 3),
                Gender = Gender.Female,
            },
            new()
            {
                Id = 2,
                FirstName = "Child1",
                LastName = "Morant",
                Relationship = Relationship.Child,
                DateOfBirth = new DateTime(2020, 6, 23),
                EmployeeId = apiResponse.Data,
                Gender = Gender.Female,
            },
            new()
            {
                Id = 3,
                FirstName = "Child2",
                LastName = "Morant",
                Relationship = Relationship.Child,
                DateOfBirth = new DateTime(2021, 5, 18),
                EmployeeId = apiResponse.Data,
                Gender = Gender.Male,
            },
            new()
            {
                Id = 4,
                FirstName = "DP",
                LastName = "Jordan",
                Relationship = Relationship.DomesticPartner,
                DateOfBirth = new DateTime(1974, 1, 2),
                EmployeeId = apiResponse.Data,
                Gender= Gender.Male,
            }
        };
            var dependentsExpected = new List<CreateDependentModel>
        {
            new()
            {
                Id = 1,
                FirstName = "Spouse",
                LastName = "Morant",
                 EmployeeId = apiResponse.Data,
                Relationship = Relationship.Spouse,
                DateOfBirth = new DateTime(1998, 3, 3),
                Gender = Gender.Female,
            },
            new()
            {
                Id = 2,
                FirstName = "Child1",
                LastName = "Morant",
                Relationship = Relationship.Child,
                DateOfBirth = new DateTime(2020, 6, 23),
                EmployeeId = apiResponse.Data,
                Gender = Gender.Female,
            },
            new()
            {
                Id = 3,
                FirstName = "Child2",
                LastName = "Morant",
                Relationship = Relationship.Child,
                DateOfBirth = new DateTime(2021, 5, 18),
                EmployeeId = apiResponse.Data,
                Gender = Gender.Male,
            },
            new()
            {
                Id = 4,
                FirstName = "DP",
                LastName = "Jordan",
                Relationship = Relationship.DomesticPartner,
                DateOfBirth = new DateTime(1974, 1, 2),
                EmployeeId = apiResponse.Data,
                Gender= Gender.Male,
            }
        };
            var counter = 0;

            foreach (var dependent in dependents)
            {
                var dependentResponse = await CreateDependent(dependent);
                var dependentResult = JsonConvert.DeserializeObject<ApiResponse<int?>>(await dependentResponse.Content.ReadAsStringAsync());
                dependent.Id = dependentResult.Data.Value;
                dependentsExpected[counter].Id = dependentResult.Data.Value;
                dependentsExpected[counter].EmployeeId = dependent.EmployeeId.Value;
                counter++;
            }

            var responseResult = await HttpClient.GetAsync("/api/v1/EmployeeDependent/GetAll");

            await responseResult.ShouldReturn(HttpStatusCode.OK, dependentsExpected.OrderByDescending(x => x.Id).ToList());
        }

        [Fact]
        //task: make test pass
        public async Task WhenAskedForADependent_ShouldReturnCorrectDependent()
        {
            var employeeresponse = await CreateEmployee(new CreateEmployeeModel { FirstName = "spouse", LastName = "Morant", DateOfBirth = new DateTime(1990, 5, 5), Salary = 100000 });
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<int?>>(await employeeresponse.Content.ReadAsStringAsync());

            var dependentResponse = await CreateDependent(new CreateDependentModel
            {
                FirstName = "Spouse",
                LastName = "Morant",
                EmployeeId = apiResponse.Data.Value,
                Relationship = Relationship.Spouse,
                DateOfBirth = new DateTime(1998, 3, 3),
                Gender = Gender.Female,
            }); 

            var dependentApiResponse = JsonConvert.DeserializeObject<ApiResponse<int?>>(await dependentResponse.Content.ReadAsStringAsync());
            var dependent = new DependentDto
            {
                Id = dependentApiResponse.Data.Value,
                FirstName = "Spouse",
                LastName = "Morant",
                EmployeeId = apiResponse.Data.Value,
                Relationship = Relationship.Spouse,
                DateOfBirth = new DateTime(1998, 3, 3),
                Gender = Gender.Female,
            };

            var response = await HttpClient.GetAsync($"/api/v1/EmployeeDependent/{dependent.Id}");

            //Asert
            await response.ShouldReturn(HttpStatusCode.OK, dependent);
        }


        [Fact]
        //task: make test pass
        public async Task WhenAskedForANonexistentDependent_ShouldReturn404()
        {
            var response = await HttpClient.GetAsync("/api/v1/EmployeeDependent/-1");

            //Asert
            await response.ShouldReturn(HttpStatusCode.NotFound);
        }
    }
}
