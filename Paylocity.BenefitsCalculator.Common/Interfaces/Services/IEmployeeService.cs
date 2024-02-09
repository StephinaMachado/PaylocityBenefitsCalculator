using Paylocity.BenefitsCalculator.Common.Dtos;
using Paylocity.BenefitsCalculator.Common.Dtos.Employee;
using Paylocity.BenefitsCalculator.Common.Models;

namespace Paylocity.BenefitsCalculator.Common.Interface.Services;

public interface IEmployeeService
{
    Task<ApiResponse<List<EmployeeDto>>> GetAllEmployee(bool active);
    Task<ApiResponse<EmployeeDto?>> GetEmployee(int employeeId);
    Task<ApiResponse<int?>> CreatetEmployee(CreateEmployeeModel employee);
    Task<ApiResponse<bool>> DeleteEmployee(int employeeId);
    Task<ApiResponse<bool>> UpdateEmployee(UpdateEmployeeModel employee);
    Task<ApiResponse<EmployeePaycheckDto>> GetEmployeePaycheck(int employeeId);
}
