using challenge.DTO;
using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompensationController : ControllerBase
    {
       private readonly ICompensationService _compensationService;
        private readonly ILogger _logger;


        public CompensationController(ICompensationService compensationService,ILogger<EmployeeController> logger)
        {
            _compensationService = compensationService;
            _logger = logger;
        }

        [HttpPost]
       public async Task<IActionResult> CreateCompensation([FromBody] CompensationDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.EmployeeID))
            {
                _logger.LogDebug("Invalid compensation request received.");
                return BadRequest("Invalid compensation request.");
            }

            try
            {
                 _logger.LogDebug($"Received compensation create request for EmployeeID: {request.EmployeeID} with Salary: {request.Salary} and EffectiveDate: {request.EffectiveDate}");

                // Check if a compensation record already exists for the employee
                var existingCompensation = await _compensationService.GetCompensationByEmployeeIdAsync(request.EmployeeID);
                
                // If the employee already has a compensation record for the same effective date, return a conflict as each employee can only have one record with a given date
                if (existingCompensation != null && existingCompensation.EffectiveDate == request.EffectiveDate)
                {
                    return Conflict("Compensation record already exists for this employee on the same date.");
                }

                // If no existing record, proceed with creation
                var compensation = await _compensationService.CreateCompensationAsync(
                    request.EmployeeID, request.Salary, request.EffectiveDate);

                return CreatedAtAction(nameof(GetCompensationByEmployeeId),
                    new { employeeId = compensation.Employee.EmployeeId }, compensation);
            }
            catch (ArgumentException ex)
            {
                _logger.LogDebug($"Error occurred while creating compensation for EmployeeID: {request.EmployeeID}: {ex.Message}");
                return NotFound(ex.Message);
            }
        }



        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetCompensationByEmployeeId(string employeeId)
        {
            _logger.LogDebug($"Received request to get compensation for EmployeeID: {employeeId}");

            var compensation = await _compensationService.GetCompensationByEmployeeIdAsync(employeeId);
            if (compensation == null)
            {
                _logger.LogDebug($"No compensation record found for EmployeeID: {employeeId}");
                return NotFound();
            }
            return Ok(compensation);
        }
    }
}
