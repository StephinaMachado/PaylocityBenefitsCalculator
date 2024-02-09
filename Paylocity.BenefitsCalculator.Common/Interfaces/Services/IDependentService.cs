
using Paylocity.BenefitsCalculator.Common.Dtos;
using Paylocity.BenefitsCalculator.Common.Dtos.Dependent;
using Paylocity.BenefitsCalculator.Common.Models;

namespace Paylocity.BenefitsCalculator.Common.Interface.Services;

public interface IDependentService
{
    Task<ApiResponse<List<DependentDto>>> GetDependentsByEmployeeId(int employeeId);
    Task<ApiResponse<List<DependentDto>>> GetAllDependedents();
    Task<ApiResponse<DependentDto>> GetDependentByIdAsync(int dependentId);
    Task<ApiResponse<int?>> CreateDependent(CreateDependentModel dependent);
    Task<ApiResponse<bool>> UpdateDependent(UpdateDependentModel dependent);
    Task<ApiResponse<bool>> DeleteDependentAsync(int dependentId);
    Task<ApiResponse<bool>> DeleteDependentByEmployeeIdAsync(int employeeId);

}
