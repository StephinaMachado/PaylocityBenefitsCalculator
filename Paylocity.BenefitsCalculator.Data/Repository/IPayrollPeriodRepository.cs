using Paylocity.BenefittsCalculator.Data.Entities;

namespace Paylocity.BenefittsCalculator.Data.Repository;

public interface IPayrollPeriodRepository
{
    Task<PayrollPeriod?> GetCurrentPayPeriod(DateTime payPeriodDate);
    Task<bool> CreatePayPeriod(DateTime startDate);
}
