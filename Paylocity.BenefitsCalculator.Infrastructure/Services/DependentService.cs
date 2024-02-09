
using Microsoft.Extensions.Logging;
using Paylocity.BenefitsCalculator.Common.Dtos;
using Paylocity.BenefitsCalculator.Common.Dtos.Dependent;
using Paylocity.BenefitsCalculator.Common.Enums;
using Paylocity.BenefitsCalculator.Common.Interface.Services;
using Paylocity.BenefitsCalculator.Common.Models;
using Paylocity.BenefittsCalculator.Data.Entities;
using Paylocity.BenefittsCalculator.Data.Repository;
using System.Text.Json;

namespace Paylocity.BenefitsCalculator.Infrastructure.Services;

public class DependentService : IDependentService
{
    private readonly IDependentRepository _repository;
    private readonly ILogger<DependentService> _logger;

    public DependentService(IDependentRepository repository, ILogger<DependentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// service creates dependent
    /// </summary>
    /// <param name="dependent"></param>
    /// <returns></returns>
    public async Task<ApiResponse<int?>> CreateDependent(CreateDependentModel dependent)
    {
        var response = new ApiResponse<int?>();

        try
        {
            if (dependent.Relationship == Relationship.Spouse || dependent.Relationship == Relationship.DomesticPartner)
            {
                var employeeDependents = await _repository.GetDependentsByEmployeeIdAsync(dependent.EmployeeId.Value);

                if (employeeDependents.Any(x => x.Relationship == Relationship.Spouse || x.Relationship == Relationship.DomesticPartner))
                {
                    _logger.LogWarning("can only have one of spouse or domestic partner");
                    response.Error = "can only have one of spouse or domestic partner";
                    return response;
                }
            }


            var dependentJson = JsonSerializer.Serialize(dependent);
            var dependentResult = JsonSerializer.Deserialize<Paylocity.BenefittsCalculator.Data.Entities.Dependent>(dependentJson);
            dependentResult.DependentStatus = DependentStatus.Active;
            response.Data = await _repository.CreateDependent(dependentResult);

            if(response.Data != null)
            {
                _logger.LogInformation("successfully added dependent");
                response.Success = true;
                response.Message = "Dependent successfully added";
            }
            else
            {
                _logger.LogError($"Error occured while adding dependent for employee Id {dependent.EmployeeId}");
                response.Success = false;
                response.Error = "Error occured";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            response.Error = ex.Message;
        }
        return response;
    }


    /// <summary>
    /// delete dependent
    /// </summary>
    /// <param name="dependentId"></param>
    /// <returns></returns>
    public async Task<ApiResponse<bool>> DeleteDependentAsync(int dependentId)
    {
        var response =   new ApiResponse<bool>();

        try
        {
            response.Data = await _repository.DeleteDependentAsync(dependentId);
            response.Success = true;
            response.Message = "Successfully deleted dependent";
            _logger.LogInformation($"successfully deleted dependent {dependentId}");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            response.Error = ex.Message;
            response.Success = false;
        }
        return response;
    }

    /// <summary>
    /// delete all employees dependent
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    public async Task<ApiResponse<bool>> DeleteDependentByEmployeeIdAsync(int employeeId)
    {
        var response = new ApiResponse<bool>();

        try
        {
            response.Data = await _repository.DeleteDependentByEmployeeIdAsync(employeeId); 
            response.Success = true;
            response.Message = $"Successfully deleted dependents from employee {employeeId}";
            _logger.LogInformation($"successfully deleted dependents from employee {employeeId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            response.Error = ex.Message;
            response.Success = false;
        }
        return response;
    }

    /// <summary>
    /// get all dependents of a given employee
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    public async Task<ApiResponse<List<DependentDto>>> GetDependentsByEmployeeId(int employeeId)
    {
        var response = new ApiResponse<List<DependentDto>>();

        try
        {
            var dependentList = await _repository.GetDependentsByEmployeeIdAsync(employeeId);
            var dependentJson = JsonSerializer.Serialize(dependentList);
            response.Data = JsonSerializer.Deserialize<List<DependentDto>>(dependentJson);
            response.Success = true;
            response.Message = "Successfully retrived dependent list";

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message); 
            response.Error = ex.Message;
        }
        return response;
    }


    /// <summary>
    /// get dependent by dependent Id
    /// </summary>
    /// <param name="dependentId"></param>
    /// <returns></returns>
    public async Task<ApiResponse<DependentDto>> GetDependentByIdAsync(int dependentId)
    {
        var response = new  ApiResponse<DependentDto>();

        try
        {
            var dependent = await _repository.GetDependentByIdAsync(dependentId);

            if(dependent == null)
            {
                response.Message = "no dependent found";
                response.Success = false;
                return response;
            }
            
            var dependentJson = JsonSerializer.Serialize(dependent);
            response.Data = JsonSerializer.Deserialize<DependentDto>(dependentJson);
            response.Success = true;
            response.Message = "Successfully retrived dependent";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            response.Success = false;
            response.Error = ex.Message;
        }
        return response;
    }


    /// <summary>
    /// Update dependent
    /// </summary>
    /// <param name="dependent"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<ApiResponse<bool>> UpdateDependent(UpdateDependentModel dependent)
    {
        if (dependent == null) throw new ArgumentNullException(nameof(dependent));
        var response = new ApiResponse<bool>();

        try
        {
            if(dependent.Relationship == Relationship.Spouse || dependent.Relationship == Relationship.DomesticPartner)
            {
                var employeeDependents = await _repository.GetDependentsByEmployeeIdAsync(dependent.EmployeeId.Value);

                if(employeeDependents.Any(x=>x.Relationship == Relationship.Spouse || x.Relationship == Relationship.DomesticPartner))
                {
                    _logger.LogWarning("can only have one of spouse or domestic partner");
                    response.Error = "can only have one of spouse or domestic partner";
                    return response;
                }
            }

            var dependentSeralised = JsonSerializer.Serialize(dependent);
            var dependentDeseralised = JsonSerializer.Deserialize<Paylocity.BenefittsCalculator.Data.Entities.Dependent>(dependentSeralised);

            response.Data = await _repository.updateDependent(dependentDeseralised);

            if(response.Data)
            {
                _logger.LogInformation("successfully added dependent");
                response.Success = true;
                response.Message = "Dependent successfully added";
            }
            else
            {
                _logger.LogError("error while adding dependent");
                response.Error = "error occured while adding dependent";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            response.Error = ex.Message;
        }
        return response;
    }


    /// <summary>
    /// gets all active dependents
    /// </summary>
    /// <returns></returns>
    public async Task<ApiResponse<List<DependentDto>>> GetAllDependedents()
    {
        var response = new ApiResponse<List<DependentDto>>();

        try
        {
            var dependent = await _repository.GetAllActiveDependentsAsync();

            if (dependent == null)
            {
                response.Message = "no dependent found";
                response.Success = false;
                return response;
            }

            var dependentJson = JsonSerializer.Serialize(dependent);
            response.Data = JsonSerializer.Deserialize<List<DependentDto>>(dependentJson);
            response.Success = true;
            response.Message = "Successfully retrived dependents";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            response.Success = false;
            response.Error = ex.Message;
        }
        return response;
    }
}
