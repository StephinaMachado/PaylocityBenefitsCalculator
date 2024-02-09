using Paylocity.BenefittsCalculator.Data.Entities;

namespace Paylocity.BenefittsCalculator.Data.Repository;

public interface IDependentRepository
{
    Task<List<Dependent>> GetDependentsByEmployeeIdAsync(int employeeId);
    Task<List<Dependent>> GetAllActiveDependentsAsync();
    Task<Dependent?> GetDependentByIdAsync(int Id);
    Task<int?> CreateDependent(Dependent dependent);
    Task<bool> updateDependent(Dependent dependent);
    Task<bool> DeleteDependentAsync(int Id);
    Task<bool> DeleteDependentByEmployeeIdAsync(int employeeId);
}
