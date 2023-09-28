using EmployeedataUsingSql.Data;
using EmployeedataUsingSql.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeedataUsingSql.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        public EmployeesController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpPost("addEmployee")]
        public async Task<IActionResult> AddEmployee(EmployeeData data)
        {
            try
            {
                if (data == null)
                {
                    return BadRequest();
                }
                var result = await _employeeService.AddEmployee(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("employeeId")]
        public async Task<IActionResult> GetEmployee(int employeId)
        {
            try
            {
                var result = await _employeeService.GetEmployeeData(employeId);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                   "Error retrieving data from the database");
            }
        }
        [HttpDelete("employeeId")]
        public async Task<IActionResult> DeleteEmployee(int employeeID)
        {
            try
            {
                var result = await _employeeService.DeleteEmployee(employeeID);
                if (result == null)
                {
                    return NotFound($"employee with id ={employeeID} is not found");
                }
                return Ok("employeedeleted succufully");

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Error retrieving data from the database");
            }
        }
        [HttpGet("GetAllemployees")]
        public async Task<IActionResult> GetallEmployees()
        {
            return Ok(await _employeeService.GetEmpolyees());
        }
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(int employeeId, EmployeeData employee)
        {
            try
            {
                var updatedEmployee = await _employeeService.UpdateEmployee(employeeId, employee);
                if (updatedEmployee == null)
                {
                    return NotFound($"Employee with id = {employeeId} is not found");
                }

                return Ok(updatedEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating data: {ex.Message}");
            }
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<EmployeeData>> SearchEmploye(string name, int? employeeId)
        {
            var result = await _employeeService.Search(name, employeeId);
            if (result.Any())
                return Ok(result);
            return NotFound();
        }


    }
}
