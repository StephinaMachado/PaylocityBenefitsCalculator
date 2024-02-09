using Paylocity.BenefitsCalculator.Common.Models;

namespace Paylocity.BenefitsCalculator.Common.Dtos.Employee;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public decimal Salary { get; set; }
    public DateTime DateOfBirth { get; set; }
    public List<CreateDependentModel>? Dependents { get; set; }
}
