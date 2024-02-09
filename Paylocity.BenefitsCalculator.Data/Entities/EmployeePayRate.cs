
using Paylocity.BenefittsCalculator.Data.Interface;

namespace Paylocity.BenefittsCalculator.Data.Entities;

public class EmployeePayRate : IEntity
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public decimal BaseSalary { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; } = DateTime.MaxValue;
    public DateTime ModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }

}
