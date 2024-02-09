using Paylocity.BenefitsCalculator.Common.Dtos.Employee;

namespace Paylocity.BenefitsCalculator.Common.Interface;

public interface IPayrollCalculationService
{
    Task<EmployeePaycheckDto?> CalculatePaycheckAsync(int employeeId);
}
