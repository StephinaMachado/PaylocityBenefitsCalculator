using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Paylocity.BenefitsCalculator.Common.Enums;
using Paylocity.BenefittsCalculator.Data.DataBaseContext;
using Paylocity.BenefittsCalculator.Data.Entities;

namespace Paylocity.BenefittsCalculator.Data.Repository;

public class DependentRepository : IDependentRepository
{
    private readonly BenefitsCalculatorContext _context;
    private readonly ILogger<DependentRepository> _logger;

    public DependentRepository(BenefitsCalculatorContext context, ILogger<DependentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates new dependent
    /// </summary>
    /// <param name="dependent"></param>
    /// <returns></returns>
    public async Task<int?> CreateDependent(Dependent dependent)
    {
        _logger.LogInformation("Creating dependent");
        _context.Dependents.Add(dependent);
        if (await _context.SaveChangesAsync() > 0)
        {
            return dependent.Id;
        }

        return null;
    }


    /// <summary>
    /// deletes dependent
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteDependentAsync(int Id)
    {
        var dependent = await _context.Dependents.Where(x => x.Id == Id).FirstOrDefaultAsync();

        if (dependent == null)
        {
            _logger.LogError("dependent id:{0} not found", Id);
            return false;
        }

        dependent.DependentStatus = DependentStatus.InActive;
        _logger.LogInformation("dependent id:{0} deleted", Id);

        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// deletes all dependents that belongs to a particular employee
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    public async Task<bool> DeleteDependentByEmployeeIdAsync(int employeeId)
    {
        var dependents = await _context.Dependents.Where(x => x.EmployeeId == employeeId && x.DependentStatus == DependentStatus.Active).ToListAsync();

        if (!dependents.Any())
        {
            _logger.LogError("No dependents found for employee Id:{0}", employeeId);
            return false;
        }

        foreach (var dependent in dependents)
        {
            dependent.DependentStatus = DependentStatus.InActive;
            _logger.LogInformation($"{dependent.Id} : deleted");
        }

        return await _context.SaveChangesAsync() > 0;
    }


    /// <summary>
    /// Gets all dependents those are active
    /// </summary>
    /// <returns></returns>
    public  async Task<List<Dependent>> GetAllActiveDependentsAsync()
    {
        return await _context.Dependents.Where(x => x.DependentStatus == DependentStatus.Active).ToListAsync();
    }


    /// <summary>
    /// get dependent by Id
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public async Task<Dependent?> GetDependentByIdAsync(int Id)
    {
        return await _context.Dependents.Where(x => x.Id == Id && x.DependentStatus == DependentStatus.Active).FirstOrDefaultAsync();
    }


    /// <summary>
    /// Gets all dependent by belonging to a particular employee
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    public async Task<List<Dependent>> GetDependentsByEmployeeIdAsync(int employeeId)
    {
        return await _context.Dependents.Where(x => x.EmployeeId == employeeId && x.DependentStatus == DependentStatus.Active).ToListAsync();

    }


    /// <summary>
    /// update dependent
    /// </summary>
    /// <param name="dependent"></param>
    /// <returns></returns>
    public async Task<bool> updateDependent(Dependent dependent)
    {
        var result = await _context.Dependents.FirstOrDefaultAsync(x => x.Id == dependent.Id && x.DependentStatus == DependentStatus.Active);

        if (result == null)
        {
            _logger.LogError($"no dependents found for employee, {dependent.EmployeeId}");
        }
        else
        {
            result.FirstName = dependent.FirstName;
            result.LastName = dependent.LastName;
            result.DependentStatus = dependent.DependentStatus;
            result.DateOfBirth = dependent.DateOfBirth;
            result.Relationship = dependent.Relationship;
            result.Gender = dependent.Gender;
        }

        return await _context.SaveChangesAsync() > 0;

    }
}
