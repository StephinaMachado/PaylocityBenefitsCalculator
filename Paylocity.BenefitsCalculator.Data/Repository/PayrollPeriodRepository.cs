using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Paylocity.BenefittsCalculator.Data.DataBaseContext;
using Paylocity.BenefittsCalculator.Data.Entities;

namespace Paylocity.BenefittsCalculator.Data.Repository;

public class PayrollPeriodRepository : IPayrollPeriodRepository
{
    private readonly BenefitsCalculatorContext _context;
    private readonly ILogger<DependentRepository> _logger;

    public PayrollPeriodRepository(BenefitsCalculatorContext context, ILogger<DependentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Create pay periods 
    /// </summary>
    /// <param name="startDate"></param>
    /// <returns></returns>
    public async Task<bool> CreatePayPeriod(DateTime startDate)
    {
        _logger.LogInformation($"creating pay periods from {startDate}");

        //create new pay periods in 14 day increment from the date provided to function
        while (startDate <= DateTime.UtcNow)
        {
            var endDate = startDate.AddDays(13).Date;
            var payPeriod = new PayrollPeriod
            {
                StartDate = startDate.Date,
                EndDate = endDate,
            };
            startDate = endDate;
            _context.PayrollPeriods.Add(payPeriod);
        }

        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// get current pay period
    /// </summary>
    /// <param name="payPeriodDate"></param>
    /// <returns></returns>
    public async Task<PayrollPeriod?> GetCurrentPayPeriod(DateTime payPeriodDate)
    {
        return await _context.PayrollPeriods.FirstOrDefaultAsync(x => x.StartDate <= payPeriodDate && x.EndDate >= payPeriodDate);
    }
}
