using Api.UnitTest;
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
using System.Threading.Tasks;
using Xunit;

namespace Paylocity.BenefitsCalculator.Api.UnitTest
{
    public class EmployeeIntegrationTest : IntegrationTest, IAsyncLifetime
    {
        [Fact]
        //task: make test pass
        public async Task WhenAskedForAllEmployees_ShouldReturnAllEmployees()
        {
            var employees = createEmployees;

            foreach (var employee in employees)
            {
                var employeeResponse = await CreateEmployee(employee);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<int?>>(await employeeResponse.Content.ReadAsStringAsync());

                if(employee.Dependents !=null && employee.Dependents.Any())
                {
                    var dependent = await HttpClient.GetAsync($"api/v1/employee/{apiResponse.Data.Value}/dependents");
                    var apiResponse1 = JsonConvert.DeserializeObject<ApiResponse<List<DependentDto>>>(await dependent.Content.ReadAsStringAsync());
                    if(apiResponse1.Data !=null)
                    {
                        foreach (var dep in apiResponse1.Data)
                        {
                            employee.Dependents.FirstOrDefault(x => x.FirstName == dep.FirstName).Id = dep.Id;
                            employee.Dependents.FirstOrDefault(x => x.FirstName == dep.FirstName).EmployeeId = apiResponse.Data.Value;
                        }
                    }
                }
                employee.Id = apiResponse.Data.Value;
            }

            var response = await HttpClient.GetAsync("/api/v1/employee");

            await response.ShouldReturn(System.Net.HttpStatusCode.OK, employees.OrderByDescending(x=>x.Id).ToList());
        }

        [Fact]
        //task: make test pass
        public async Task WhenAskedForAnEmployee_ShouldReturnCorrectEmployee()
        {
            var response1 = await CreateEmployee( createEmployees.FirstOrDefault());
            var ApiResponse = JsonConvert.DeserializeObject<ApiResponse<int?>>(await response1.Content.ReadAsStringAsync());
            var employee = createEmployees.FirstOrDefault();
            employee.Id = ApiResponse.Data.Value;

            var response = await HttpClient.GetAsync($"/api/v1/employee/{employee.Id}");

            //Assert
            await response.ShouldReturn(HttpStatusCode.OK, employee);
        }

        [Fact]
        //task: make test pass
        public async Task WhenAskedForANonexistentEmployee_ShouldReturn404()
        {

            var response = await HttpClient.GetAsync("/api/v1/employee/-1");

            await response.ShouldReturn(HttpStatusCode.NotFound);
        }



        private List<CreateEmployeeModel> createEmployees => new List<CreateEmployeeModel>
        {
            new()
            {
                FirstName = "Employee1",
                LastName = "LastName1",
                Salary = 68000,
                DateOfBirth = new DateTime(1988, 3, 4),
            },
            new()
            {
                FirstName = "Employee2",
                LastName = "LastName2",
                Salary = 68000,
                DateOfBirth = new DateTime(1988, 3, 4),
            },
            new()
            {
                FirstName = "Spouse", LastName = "Morant", DateOfBirth = new DateTime(1990, 5, 5), Salary = 100000,
                Dependents = new List<CreateDependentModel>
                {
                    new()
                    {
                        FirstName = "Husband",
                        LastName = "Morant",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1990, 1, 12),
                        Gender = Gender.Male,
                    },
                    new()
                    {
                        FirstName = "Child",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2018, 1, 12),
                        Gender = Gender.Male,
                    },
                    new()
                    {
                        FirstName = "Child2",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2018, 1, 12),
                        Gender = Gender.Male,
                    },
                    new()
                    {
                        FirstName = "Child3",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2018, 1, 12),
                        Gender = Gender.Male,
                    }
                }
            }
        };

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
