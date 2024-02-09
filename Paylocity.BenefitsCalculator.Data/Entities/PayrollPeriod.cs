
using Paylocity.BenefittsCalculator.Data.Interface;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paylocity.BenefittsCalculator.Data.Entities;

public class PayrollPeriod : IEntity
{
    public int Id { get; set; }
    [ForeignKey("Employee")]
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
