using Microsoft.Extensions.Logging;
using Paylocity.BenefitsCalculator.Common.Dtos;
using Paylocity.BenefitsCalculator.Common.Dtos.Employee;
using Paylocity.BenefitsCalculator.Common.Enums;
using Paylocity.BenefitsCalculator.Common.Interface;
using Paylocity.BenefitsCalculator.Common.Interface.Services;
using Paylocity.BenefitsCalculator.Common.Models;
using Paylocity.BenefittsCalculator.Data.Entities;
using Paylocity.BenefittsCalculator.Data.Repository;
using System.Text.Json;

namespace Paylocity.BenefitsCalculator.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayrollCalculationService _payrollService;
    private readonly ILogger<DependentService> _logger;

    public EmployeeService(IEmployeeRepository employeeRepository, IPayrollCalculationService payrollService, ILogger<DependentService> logger)
    {
        _employeeRepository = employeeRepository;
        _payrollService = payrollService;
        _logger = logger;
    }

    /// <summary>
    /// create new employee
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    public async Task<ApiResponse<int?>> CreatetEmployee(CreateEmployeeModel employee)
    {
        var response = new ApiResponse<int?>();
        if(employee.Dependents !=null && employee.Dependents.Count(x=>x.Relationship == Relationship.Spouse || x.Relationship == Relationship.DomesticPartner) > 1)
        {
            _logger.LogWarning("Employee can only have one of spouse or domestic partner");
            response.Success = false;
            return response;
        }
        try
        {
            var employeeResult = new Employee
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DateOfBirth = employee.DateOfBirth,
                EmployeeStatus = EmployeeStatus.Active,
                EmployeePayRates = new List<EmployeePayRate>()
                {
                    new EmployeePayRate
                    {
                        BaseSalary = employee.Salary,
                        StartDate = employee.StartDate,
                    }
                }
            };

            if(employee.Dependents?.Any() ?? false)
            {
                employeeResult.Dependents = employee.Dependents.Select(x=> new Dependent
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,  
                    DateOfBirth= x.DateOfBirth,
                    DependentStatus = DependentStatus.Active,
                    Gender = x.Gender,
                    Relationship = x.Relationship

                }).ToList();
            }

            response.Data = await _employeeRepository.CreateEmployee(employeeResult);

            if(response.Data != null)
            {
                response.Success = true;
                response.Message = "Employee created";
                _logger.LogInformation("Employee created");
            }
            else
            {
                response.Success = false;
                response.Message = "unable to create employee";
            }
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
            _logger.LogError(ex, ex.Message);
        }

        return response;
    }

    
    /// <summary>
    /// delete employee
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    public async Task<ApiResponse<bool>> DeleteEmployee(int employeeId)
    {
        var response = new ApiResponse<bool>();

        try
        {
            response.Data = await _employeeRepository.DeleteEmployee(employeeId);
            response.Success = true;
            response.Message = "Employee deleted";
            _logger.LogInformation($"Employee {employeeId} deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            response.Error = ex.Message;
            response.Message = "Error occured";
        }
        return response;
    }

    /// <summary>
    /// get all employees
    /// </summary>
    /// <param name="active"></param>
    /// <returns></returns>
    public async Task<ApiResponse<List<EmployeeDto>>> GetAllEmployee(bool active)
    {
        var response = new ApiResponse<List<EmployeeDto>>();

        try
        {
            var employeeList = await _employeeRepository.GetEmployees(active);

            _logger.LogInformation("employees retrived");

            var employeeJson = JsonSerializer.Serialize(employeeList);
            response.Data = JsonSerializer.Deserialize<List<EmployeeDto>>(employeeJson);

            foreach (var empl in response.Data)
            {
                var now = DateTime.UtcNow.Date;
                var employee = employeeList.First(x => x.Id == empl.Id);
                empl.Salary = employee.EmployeePayRates.FirstOrDefault(x => x.StartDate.Date <= now && x.EndDate >= now)?.BaseSalary ?? 0;

                if (!empl.Dependents?.Any() ?? false)
                {
                    empl.Dependents = null;
                }
            }
            response.Success = true;
            response.Message = "employees retrived";
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
            _logger.LogError(ex, ex.Message);
        }

        return response;
    }


    /// <summary>
    /// get employee given employee Id
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    public async Task<ApiResponse<EmployeeDto?>> GetEmployee(int employeeId)
    {
        var response = new ApiResponse<EmployeeDto?>();

        try
        {
            var employee = await _employeeRepository.GetEmployeesByIdAsync(employeeId);

            if (employee == null)
            {
                _logger.LogWarning($"Employee {employeeId} does not exist");
                response.Success = false;
                response.Message = "Employee does not exist";
                return response;
            }
            _logger.LogInformation($"Employee retrived");

            var employeeJson = JsonSerializer.Serialize(employee);
            response.Data = JsonSerializer.Deserialize<EmployeeDto>(employeeJson);

            var now = DateTime.UtcNow.Date;

            if (!response.Data?.Dependents?.Any() ?? false)
            {
                response.Data.Dependents = null;
            }
            response.Data.Salary = employee.EmployeePayRates.FirstOrDefault(x => x.StartDate.Date <= now && x.EndDate >= now)?.BaseSalary ?? 0;
            response.Success = true;
            response.Message = "Successfully retrived";
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
            _logger.LogError(ex, ex.Message);
        }

        return response;
    }

    
    /// <summary>
    /// get employee pay check
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    public async Task<ApiResponse<EmployeePaycheckDto>> GetEmployeePaycheck(int employeeId)
    {
        var response = new ApiResponse<EmployeePaycheckDto>();

        try
        {
            var employee = await _employeeRepository.GetEmployeesByIdAsync(employeeId);

            var data = await _payrollService.CalculatePaycheckAsync(employeeId);
            if (data == null)
            {
                response.Success = false;
                response.Message = "Employee not found";
                _logger.LogInformation($"Employee {employeeId} not found");
            }
            else
            {
                response.Data = data;
                response.Data.FirstName = employee?.FirstName;
                response.Data.LastName = employee?.LastName;
                response.Data.Id = employee.Id;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"{ex.Message}", ex);
            response.Error = ex.Message;
            response.Message = ex.Message;
        }

        return response;
    }


    /// <summary>
    /// update employee
    /// </summary>
    /// <param name="employeeModel"></param>
    /// <returns></returns>
    public async Task<ApiResponse<bool>> UpdateEmployee(UpdateEmployeeModel employeeModel)
    {
        var response = new ApiResponse<bool>();
        try
        {
            var updateEmployee = await _employeeRepository.UpdateEmployee(employeeModel);

            if(updateEmployee)
            {
                _logger.LogError("Unable to save employee");
                response.Error = "An error occured while saving employee.";
                return response;
            }

            var employee = await _employeeRepository.GetEmployeesByIdAsync(employeeModel.Id);

            if(employee == null)
            {
                response.Success = false;
                response.Message = "Unable to find employee";
                return response;
            }

            var now = DateTime.UtcNow;
            EmployeePayRate employeePayRate = employee.EmployeePayRates.First(x=> x.StartDate > now && x.EndDate < now);

            if(employeePayRate.BaseSalary != employeeModel.Salary)
            {
                _logger.LogInformation("Pay updated for employee");
                await _employeeRepository.UpdateEmployeePayRate(employeeModel.Salary, employeePayRate.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"{ex.Message}", ex);
            response.Error = ex.Message;
            response.Message = ex.Message;
        }

        return response;
    }
}
