using Paylocity.BenefitsCalculator.Common.Enums;

namespace Paylocity.BenefitsCalculator.Common.Models
{
    public class UpdateDependentModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? EmployeeId { get; set; }
        public Relationship Relationship { get; set; }
        public Gender Gender { get; set; }
        public DependentStatus DependentStatus { get; set; }
        public DateTime DateOfBirth { get; set; }

    }
}
