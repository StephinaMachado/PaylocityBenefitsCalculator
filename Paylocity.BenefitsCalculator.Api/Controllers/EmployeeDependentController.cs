using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Paylocity.BenefitsCalculator.Common.Interface.Services;
using Paylocity.BenefitsCalculator.Common.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Paylocity.BenefitsCalculator.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeeDependentController : ControllerBase
    {
        private readonly IDependentService _dependentService;

        public EmployeeDependentController(IDependentService dependentService)
        {
            _dependentService = dependentService;
        }

        /// <summary>
        /// Get Dependents by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Get Dependents by Id")]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var response = await _dependentService.GetDependentByIdAsync(id);

            if (!response.Success)
            {
                if (string.IsNullOrEmpty(response.Error))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(response.Error);
                }
            }

            return Ok(response);
        }


        /// <summary>
        /// Get All Active dependents
        /// </summary>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Get All Active dependents")]
        [HttpGet()]
        public async Task<ActionResult> GetAll()
        {
            var response = await _dependentService.GetAllDependedents();

            if (!response.Success)
            {
                if (string.IsNullOrEmpty(response.Error))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(response.Error);
                }
            }

            return Ok(response);
        }


        /// <summary>
        /// Add new dependent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Add dependent")]
        [HttpPost("")]
        public async Task<ActionResult> CreateDependent(CreateDependentModel model)
        {
            var response = await _dependentService.CreateDependent(model);
            return Ok(response);
        }


        /// <summary>
        /// Update dependent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Update dependent")]
        [HttpPut("")]
        public async Task<ActionResult> UpdateDependent(UpdateDependentModel model)
        {
            var response = await _dependentService.UpdateDependent(model);
            return Ok(response);
        }



        /// <summary>
        /// Delete dependent by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SwaggerOperation(Summary = "Delete dependents")]
        [HttpPut("{id}")]
        public async Task<ActionResult> DeleteDependentById([FromRoute] int id)
        {
            var response = await _dependentService.DeleteDependentAsync(id);
            return Ok(response);
        }
    }
}
