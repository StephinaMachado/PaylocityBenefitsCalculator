

using Paylocity.BenefitsCalculator.Common.Enums;
using Paylocity.BenefittsCalculator.Data.Interface;
using System.ComponentModel.DataAnnotations;

namespace Paylocity.BenefittsCalculator.Data.Entities;

public class Employee : IEntity
{
    [Key]
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }

    public EmployeeStatus EmployeeStatus { get; set; }

    public ICollection<Dependent> Dependents { get; set; }

    public ICollection<EmployeePayRate> EmployeePayRates { get; set; }
    public Relationship Relationship { get; set; }
    
    public Gender Gender { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
