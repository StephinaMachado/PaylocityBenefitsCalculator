using Microsoft.Extensions.Logging;
using Paylocity.BenefitsCalculator.Common.Dtos.Employee;
using Paylocity.BenefitsCalculator.Common.Enums;
using Paylocity.BenefitsCalculator.Common.Interface;
using Paylocity.BenefittsCalculator.Data.Entities;
using Paylocity.BenefittsCalculator.Data.Repository;
using System.Runtime.CompilerServices;

namespace Paylocity.BenefitsCalculator.Infrastructure.Services;

public class PayrollCalculationService: IPayrollCalculationService
{
    private readonly ILogger<PayrollCalculationService> _logger;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayrollPeriodRepository _payrollPeriodRepository;
    private const int TotalPayChecks = 26;
    public PayrollCalculationService(IEmployeeRepository employeeRepository, IPayrollPeriodRepository payrollPeriodRepository, ILogger<PayrollCalculationService> logger)
    {
        _logger= logger;
        _employeeRepository= employeeRepository;
        _payrollPeriodRepository= payrollPeriodRepository;
    }


    /// <summary>
    /// calculate employee paycheck
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    public async Task<EmployeePaycheckDto?> CalculatePaycheckAsync(int employeeId)
    {
        var response = new EmployeePaycheckDto();

        var employee = await _employeeRepository.GetEmployeesByIdAsync(employeeId);

        if(employee == null || employee.Dependents.Count(x=>x.Relationship == Relationship.Spouse || x.Relationship == Relationship.DomesticPartner) > 1)
        {
            return null;
        }

        //get active pay period
        var currentPayPeriod = await _payrollPeriodRepository.GetCurrentPayPeriod(DateTime.UtcNow);

        if(currentPayPeriod == null)
        {
            // there is no current payperiod create new pay periods in 14 day increment from the date provided to function
            await _payrollPeriodRepository.CreatePayPeriod(new DateTime(2023, 1, 2));
            currentPayPeriod = await _payrollPeriodRepository.GetCurrentPayPeriod(DateTime.UtcNow);
        }

        if(currentPayPeriod == null)
        {
            return null;
        }

        response.StartDate = currentPayPeriod.StartDate;
        response.EndDate = currentPayPeriod.EndDate;

        //get employee salary
        var employeePayrate = employee.EmployeePayRates.Where(x => x.StartDate <= currentPayPeriod.EndDate && x.EndDate >= currentPayPeriod.StartDate).FirstOrDefault();
        var hasCalculatedBenefits = false;

        //Calculates Benefits
        if(!hasCalculatedBenefits)
        {
            response.BenefitDeductions = CalculateBenefitsAmountForPayPeriod(currentPayPeriod, employee);
            hasCalculatedBenefits = true;
        }

        //Calculate bi-weekly pay
        response.GrossPay += employeePayrate.BaseSalary / TotalPayChecks;

        //if employee salary is over 80000, 2% should be added to additional deduction                
        if(employeePayrate.BaseSalary > 80000) {
            // (2%  * Base salary) / 26 => Additional dedcution bi-weekly
            response.AdditionalDeductions += (employeePayrate.BaseSalary * 2 / (TotalPayChecks * 100)); 
        }
          
        response.NetPay = response.GrossPay - (response.BenefitDeductions + response.AdditionalDeductions);
        return response;
    }


    /// <summary>
    /// CAlculate benefits for a period
    /// </summary>
    /// <param name="period"></param>
    /// <param name="employee"></param>
    /// <returns></returns>
    private decimal CalculateBenefitsAmountForPayPeriod(PayrollPeriod period, Employee employee)
    {
        double benefitsForDependentBelow50 = 600.0;
        double benefitsForDependentAbove50 = 800.0;
        double employeeBaseBenefit = 1000.0;
        //Count of Dependents who are  above 50yrs
        var dependentsAbove50 = employee.Dependents.Where(x => IsDependentAgeAbove50(x.DateOfBirth)).Count();
        //Count of Dependents who are  below 50yrs
        var dependentsBelow50 = employee.Dependents.Count - dependentsAbove50;

        //check if payperiod is split across more than one month. 
        var numberOfDaysInEachMonth = GetNumberOfDaysInEachMonth(period.StartDate, period.EndDate);
        decimal totalBenefitCost = 0;

        //loop each month to calculate benefit cost
        foreach (var daysInMonth in numberOfDaysInEachMonth)
        {
            //get benefits for a day 
            decimal benefitsForDependentBelow50PerDay = (decimal)(benefitsForDependentBelow50 / daysInMonth.Value);
            decimal benefitsForDependentAbove50PerDay = (decimal)(benefitsForDependentAbove50 / daysInMonth.Value);
            decimal employeeBaseBenefitPerDay = (decimal)(employeeBaseBenefit / daysInMonth.Value);

            //get the month
            var payPeriodStartDateMonth = period.StartDate.ToString("MMMM yyyy");
            var payPeriodEndDateMonth = period.EndDate.ToString("MMMM yyyy");

            decimal totalWorkingDays = 0;

            //If the payperiod is in the same month
            if (payPeriodStartDateMonth == payPeriodEndDateMonth)
            {
                //If the payperiod start and end dates are in same month. 
                totalWorkingDays = (decimal) (period.EndDate.Date - period.StartDate.Date).TotalDays + 1;
            }
            else if(daysInMonth.Key == payPeriodStartDateMonth)
            {
                //Calculate the days from payrollStartDate to end of same month
                totalWorkingDays = (decimal)(new DateTime(period.StartDate.Year, period.StartDate.Month, (int)(daysInMonth.Value)) - period.StartDate).TotalDays + 1;
            }
            else
            {
                //Calculate the days from start of the month to payrollEndDate
                totalWorkingDays = (decimal)(period.EndDate - new DateTime(period.EndDate.Year, period.EndDate.Month, 1)).TotalDays + 1;
            }

            totalBenefitCost +=
                ((dependentsAbove50 * benefitsForDependentAbove50PerDay * totalWorkingDays) +
                (dependentsBelow50 * benefitsForDependentBelow50PerDay * totalWorkingDays) +
                (employeeBaseBenefitPerDay * totalWorkingDays));
        }
        return totalBenefitCost;
    }


    /// <summary>
    /// checks is a particular dependent is above the age of 50
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private bool IsDependentAgeAbove50(DateTime date)
    {
        var today = DateTime.UtcNow;

        var age = today.Year - date.Year;

        if(today.Month < date.Month || (today.Month == date.Month && today.Day < date.Day))
        {
            age--;
        }
        return age > 50;
    }


    /// <summary>
    /// calculates the number of days if the pay period falls within two months and gets the days in each month
    /// </summary>
    /// <param name="PayPeriodStart"></param>
    /// <param name="PayPeriodEnd"></param>
    /// <returns></returns>
    private Dictionary<string, double> GetNumberOfDaysInEachMonth(DateTime PayPeriodStart, DateTime PayPeriodEnd)
    {
        var daysInEachMonth = new Dictionary<string, double>();

        DateTime currentDate = PayPeriodStart;

        while(currentDate.Month <= PayPeriodEnd.Month)
        {
            string monthYear = currentDate.ToString("MMMM yyyy");
            int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            if(!daysInEachMonth.ContainsKey(monthYear))
            {
                daysInEachMonth.Add(monthYear, daysInMonth);
            }
            currentDate = currentDate.AddMonths(1);
        }

        return daysInEachMonth;
    }


    /// <summary>
    /// calculates total working days
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    private int GetTotalWorkingDays(DateTime startDate, DateTime endDate)
    {
        int days = 0;
        for(var date = startDate; date<=endDate; date = date.AddDays(1))
        {
            if(date.DayOfWeek !=DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday)
            {
                days++;
            }
        }

        return days;
    }
}
