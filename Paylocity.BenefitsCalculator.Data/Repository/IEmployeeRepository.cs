using Paylocity.BenefitsCalculator.Common.Models;
using Paylocity.BenefittsCalculator.Data.Entities;

namespace Paylocity.BenefittsCalculator.Data.Repository;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetEmployees(bool active);
    Task<Employee?> GetEmployeesByIdAsync(int id);
    Task<int?> CreateEmployee(Employee employee);
    Task<bool> UpdateEmployee(UpdateEmployeeModel employee);
    Task<bool> UpdateEmployeePayRate(decimal newPayrate, int Id);
    Task<bool> DeleteEmployee(int Id);
}
