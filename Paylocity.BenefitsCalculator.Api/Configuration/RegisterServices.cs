
using Microsoft.EntityFrameworkCore;
using Paylocity.BenefitsCalculator.Common.Interface;
using Paylocity.BenefitsCalculator.Common.Interface.Services;
using Paylocity.BenefitsCalculator.Infrastructure.Services;
using Paylocity.BenefittsCalculator.Data.DataBaseContext;
using Paylocity.BenefittsCalculator.Data.Repository;

namespace Paylocity.BenefitsCalculator.Api.Configuration
{
    public static class RegisterServices
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddDbContextPool<BenefitsCalculatorContext>(options => options.UseSqlite("Data Source=DBEmployeeBenefits.db"));
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<IDependentService, DependentService>();
            services.AddTransient<IDependentRepository, DependentRepository>();
            services.AddTransient<IPayrollPeriodRepository, PayrollPeriodRepository>();
            services.AddTransient<IPayrollCalculationService, PayrollCalculationService>();
        }
    }
}
