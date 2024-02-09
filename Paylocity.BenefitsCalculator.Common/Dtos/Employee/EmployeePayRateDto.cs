namespace Paylocity.BenefitsCalculator.Common.Dtos.Employee;

public class EmployeePayRateDto
{
    public int Id { get; set; }
    public decimal BaseSalary { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; } = DateTime.MaxValue;
}
