
using Paylocity.BenefittsCalculator.Data.Interface;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paylocity.BenefittsCalculator.Data.Entities;

public class EmployeePayPeriod : IEntity
{
    public int Id { get; set; }

    [ForeignKey("Employee")]
    public int EmployeeId { get; set; }

    [ForeignKey("PayrollPeriod")]
    public int PayrollPeriodId { get; set; }

    public decimal BaseAmount { get; set; }
    public decimal BenefitsAmount { get; set; }
    public decimal AdditionalBenefitCost { get; set; }
    public decimal TotalAmount { get; set; }
    public Employee Employee { get; set; }
    public PayrollPeriod PayrollPeriod { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
