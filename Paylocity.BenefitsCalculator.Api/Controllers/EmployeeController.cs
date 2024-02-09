using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paylocity.BenefitsCalculator.Common.Interface.Services;
using Paylocity.BenefitsCalculator.Common.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Paylocity.BenefitsCalculator.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDependentService _dependentService;

        public EmployeeController(IEmployeeService employeeService, IDependentService dependentService)
        {
            _employeeService = employeeService;
            _dependentService = dependentService;
        }

        /// <summary>
        /// Get employee by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Get Employee by Id")]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get([FromRoute] int id)
        {
            var response = await _employeeService.GetEmployee(id);

            if(!response.Success)
            {
                if(string.IsNullOrEmpty(response.Error))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(response);
                }
            }
            return Ok(response);
        }

        /// <summary>
        /// Get all dependents by employee id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Get Employee dependents")]
        [HttpGet("{id}/dependents")]
        public async Task<ActionResult> GetEmployeeDependent([FromRoute] int id)
        {
            var response = await _dependentService.GetDependentsByEmployeeId(id);
            return Ok(response);
        }

        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Get All Employee List")]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var response = await _employeeService.GetAllEmployee(true);
            return Ok(response);
        }

        /// <summary>
        /// Delete Employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Delete Employee")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployee([FromRoute] int id)
        {
            var response = await _employeeService.DeleteEmployee(id);
            return Ok(response);
        }

        /// <summary>
        /// Create Employee
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Create Employee")]
        [HttpPost]
        public async Task<ActionResult> CreateEmployee(CreateEmployeeModel model)
        {
            var response = await _employeeService.CreatetEmployee(model);
            return Ok(response);
        }

        /// <summary>
        /// Update employee
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Update Employee")]
        [HttpPut]
        public async Task<ActionResult> UpdateEmployee(UpdateEmployeeModel model)
        {
            var response = await _employeeService.UpdateEmployee(model);
            return Ok(response);
        }

        /// <summary>
        /// Get employee pay
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Get Employee payPeriod")]
        [HttpGet("{id}/payPeriod")]
        public async Task<ActionResult> GetEmployeePayPeriod(int id)
        {
            var response = await _employeeService.GetEmployeePaycheck(id);
            return Ok(response);
        }
    }
}
