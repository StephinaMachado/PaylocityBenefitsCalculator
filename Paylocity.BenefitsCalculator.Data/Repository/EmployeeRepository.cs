using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Paylocity.BenefitsCalculator.Common.Enums;
using Paylocity.BenefitsCalculator.Common.Models;
using Paylocity.BenefittsCalculator.Data.DataBaseContext;
using Paylocity.BenefittsCalculator.Data.Entities;

namespace Paylocity.BenefittsCalculator.Data.Repository;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly BenefitsCalculatorContext _context;
    private readonly ILogger<DependentRepository> _logger;

    public EmployeeRepository(BenefitsCalculatorContext context, ILogger<DependentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates Employee
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    public async Task<int?> CreateEmployee(Employee employee)
    {
        int? Id = null;
        _logger.LogInformation("creating Employee");

        await _context.Employees.AddAsync(employee);

        if (await _context.SaveChangesAsync() > 0)
        {
            Id = employee.Id;
        }
        return Id;
    }


    /// <summary>
    /// Deletes employee
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteEmployee(int Id)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == Id && x.EmployeeStatus == EmployeeStatus.Active);
        if (employee != null)
        {
            employee.EmployeeStatus = EmployeeStatus.InActive;
            _logger.LogInformation($"deleting employee {Id}");
            return await _context.SaveChangesAsync() > 0;
        }
        _logger.LogError($"unable to find employee {Id}");
        return false;
    }


    /// <summary>
    /// Gets all employees
    /// </summary>
    /// <param name="active"></param>
    /// <returns></returns>
    public async Task<List<Employee>> GetEmployees(bool active)
    {
        return await _context.Employees.Include(x => x.Dependents.Where(d => d.DependentStatus == DependentStatus.Active))
            .Include(x => x.EmployeePayRates)
            .Where(x => x.EmployeeStatus == EmployeeStatus.Active)
            .OrderByDescending(x => x.Id)
            .ToListAsync() ?? new List<Employee>();
    }


    /// <summary>
    /// gets a particular employee
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Employee?> GetEmployeesByIdAsync(int id)
    {
        return await _context.Employees.Where(x => x.Id == id && x.EmployeeStatus == EmployeeStatus.Active)
            .Include(x => x.Dependents)
            .Include(x => x.EmployeePayRates)
            .AsNoTracking().FirstOrDefaultAsync();
    }


    /// <summary>
    /// update employee
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public async Task<bool> UpdateEmployee(UpdateEmployeeModel employee)
    {
        _logger.LogInformation($"updating employee {employee.Id}");

        var employeeResult = await _context.Employees.FirstOrDefaultAsync(x => x.Id == employee.Id);
        if (employeeResult == null)
        {
            _logger.LogError($"no employee with id {employee.Id} found");
            throw new InvalidDataException();
        }
        employeeResult.FirstName = employee.FirstName;
        employeeResult.LastName = employee.LastName;
        employeeResult.DateOfBirth = employee.DateOfBirth;
        employeeResult.EmployeeStatus = employee.EmployeeStatus;

        return await _context.SaveChangesAsync() > 0;
    }


    /// <summary>
    /// updates only the employee pay rate
    /// </summary>
    /// <param name="newPayrate"></param>
    /// <param name="Id"></param>
    /// <returns></returns>
    public async Task<bool> UpdateEmployeePayRate(decimal newPayrate, int Id)
    {
        var payRate = await _context.EmployeePayRates.FirstAsync(x => x.Id == Id);

        payRate.EndDate = DateTime.UtcNow;

        var newEmployeePayRate = new EmployeePayRate
        {
            BaseSalary = newPayrate,
            EmployeeId = payRate.EmployeeId,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.MaxValue
        };

        _context.EmployeePayRates.Add(newEmployeePayRate);

        return await _context.SaveChangesAsync() > 0;
    }
}
