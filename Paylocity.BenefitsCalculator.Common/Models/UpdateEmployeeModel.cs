
using Paylocity.BenefitsCalculator.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Paylocity.BenefitsCalculator.Common.Models
{
    public class UpdateEmployeeModel
    {
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 1)]
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 1)]
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        public decimal Salary { get; set; }
        public DateTime DateOfBirth { get; set; }

        public EmployeeStatus EmployeeStatus { get; set; }
        public List<CreateDependentModel>? Dependents { get; set; }
    }
}
