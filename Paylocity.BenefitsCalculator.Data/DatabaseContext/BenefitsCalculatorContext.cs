using Microsoft.EntityFrameworkCore;
using Paylocity.BenefittsCalculator.Data.Entities;
using Paylocity.BenefittsCalculator.Data.Interface;

namespace Paylocity.BenefittsCalculator.Data.DataBaseContext
{
    public class BenefitsCalculatorContext : DbContext
    {
        public BenefitsCalculatorContext(DbContextOptions<BenefitsCalculatorContext> options) : base(options)
        {
                
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Dependent> Dependents { get; set; }
        public DbSet<EmployeePayPeriod> EmployeePayPeriods { get; set; }
        public DbSet<EmployeePayRate> EmployeePayRates { get; set; }
        public DbSet<PayrollPeriod> PayrollPeriods { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken token = new  CancellationToken())
        {
            var now = DateTime.UtcNow;

            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if(changedEntity.Entity is IEntity entity)
                {
                    if(changedEntity.State == EntityState.Added)
                    {
                        entity.CreatedDate = now;
                        entity.ModifiedDate = now;
                    } else if(changedEntity.State == EntityState.Modified)
                    {
                        entity.ModifiedDate = now;
                    }
                    
                }
            }
            return await base.SaveChangesAsync(token);
        }

    }
}
