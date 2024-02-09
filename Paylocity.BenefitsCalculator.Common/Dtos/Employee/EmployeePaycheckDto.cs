namespace Paylocity.BenefitsCalculator.Common.Dtos.Employee;

public class EmployeePaycheckDto
{
    public int Id { get; set; }
    public decimal NetPay { get; set; }
    public decimal BenefitDeductions { get; set; }
    public decimal AdditionalDeductions { get; set; }
    public decimal GrossPay { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
