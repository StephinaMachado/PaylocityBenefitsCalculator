
using Paylocity.BenefitsCalculator.Common.Enums;

namespace Paylocity.BenefitsCalculator.Common.Models
{
    public class CreateDependentModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? EmployeeId { get; set; }
        public Relationship Relationship { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
