
using Paylocity.BenefitsCalculator.Common.Enums;
using Paylocity.BenefittsCalculator.Data.Interface;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paylocity.BenefittsCalculator.Data.Entities;

public class Dependent : IEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }

    [ForeignKey("Employee")]
    public int EmployeeId { get; set; }

    public Relationship Relationship { get; set; }
    public DependentStatus DependentStatus { get; set; }
    public Gender Gender { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
