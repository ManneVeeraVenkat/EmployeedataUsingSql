using EmployeedataUsingSql.Data;
using EmployeedataUsingSql.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections;

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

        [HttpPost("uploadEmployeeWithProfilePic")]
        public async Task<IActionResult> UploadEmployeeWithProfilePic([FromForm] EmployeeData data, IFormFile file)
        {
            try
            {
                if (data == null || file == null || file.Length == 0)
                {
                    return BadRequest("Invalid request data");
                }

                // Generate a unique blob name using the employee's ID
                string blobName = $"{data.employeeId}{Path.GetExtension(file.FileName)}";

                // Upload the profile picture with the generated blob name
                var imageUrl = await _employeeService.UploadProfilePic(file, blobName);


                // Set the profile picture URL in the employee data
                //data.ProfilePicturePath = imageUrl;

                // Add the employee
                var result = await _employeeService.AddEmployee(data);
                var response = new
                {
                    EmployeeData = result,
                    ProfilePicture = imageUrl,

                };


                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        //{
        //    try
        //    {
        //        var imageUrl = await _employeeService.UploadProfilePic(file);
        //        return Ok(new { ImageUrl = imageUrl });
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        //[HttpPost("addEmployee")]
        //public async Task<IActionResult> AddEmployee(EmployeeData data)
        //{
        //    try
        //    {
        //        if (data == null)
        //        {
        //            return BadRequest();
        //        }
        //        var result = await _employeeService.AddEmployee(data);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpGet("employeeDetails")]
        public async Task<IActionResult> GetEmployeeDetails(int employeId)
        {
            try
            {
                // Fetch employee data
                var employeeData = await _employeeService.GetEmployeeData(employeId);

                // If employee data is not found, return NotFound
                if (employeeData == null)
                {
                    return NotFound();
                }

                string[] imageExtensions = new string[] { ".jpg", ".png", ".jpeg", ".gif" }; // Add more as needed

                foreach (string imageExtension in imageExtensions)
                {
                    string blobName = $"{employeId}{imageExtension}";

                    var result = await _employeeService.GetProfilePicAsync(blobName);

                    if (result != null)
                    {


                        var memoryStream = new MemoryStream();
                        await result.CopyToAsync(memoryStream);
                        var byteArray = memoryStream.ToArray();

                        var response = new
                        {
                            EmployeeData = employeeData,
                            ProfilePicture = byteArray,
                        };

                        return Ok(response); // Return both employee data and the profile picture
                    }
                }

                // If no profile picture is found for any extension, return just the employee data
                return Ok(employeeData);
            }
            catch (Exception ex)
            {
                // Add an error message to the log
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
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
        //[HttpGet("employeeprofile")]
        //public async Task<IActionResult> GetEmployeeProfilePic(string employeId)
        //{
        //    // Define a list of common image extensions to try
        //    string[] imageExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" }; // Add more as needed

        //    foreach (string extension in imageExtensions)
        //    {
        //        string blobName = $"{employeId}{extension}";

        //        try
        //        {
        //            var result = await _employeeService.GetProfilePicAsync(blobName);
        //            if (result != null)
        //            {
        //                return Ok(result);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(StatusCodes.Status500InternalServerError,
        //                           "Error retrieving data from the database");
        //        }
        //    }

        //    // If none of the extensions were found, return a Not Found response
        //    return NotFound();
        //}

        //public async Task<IActionResult> GetEmployeeProfilePic(string employeId)
        //{

        //    string blobName = $"{employeId}.jpg";

        //    try
        //    {
        //        var result = await _employeeService.DownloadFileAsync(blobName);
        //        if (result == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //                           "Error retrieving data from the database");
        //    }
        //}
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
        //[HttpDelete("employeeprofie")]
        //public async Task<IActionResult> DeleteFile( string employeeid)
        //{
        //    bool isDeleted = await _employeeService.DeleteProfilePic(employeeid);

        //    if (isDeleted)
        //        return Ok(new { Message = "File deleted successfully" });
        //    else
        //        return NotFound("File not found or could not be deleted");
        //}
        [HttpDelete("deleteEmployee")]
        public async Task<IActionResult> DeleteEmployeeWithProfilePicAndData(int employeeId)
        {
            bool isProfilePicDeleted = false;
            try
            {
                // Delete the employee data
                var employeeResult = await _employeeService.DeleteEmployee(employeeId);

                // Delete the employee profile picture
                string[] imageExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                foreach (string imageExtension in imageExtensions)
                {
                    string blobName = $"{employeeId}{imageExtension}";
                    bool isDeleted = await _employeeService.DeleteProfilePic(blobName);
                    if (isDeleted)
                    {
                        isProfilePicDeleted = true;
                    }

                }

                if (employeeResult == null)
                {
                    return NotFound($"Employee with ID {employeeId} not found");
                }

                if (isProfilePicDeleted)
                {
                    return Ok("Employee and associated data deleted successfully");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting employee profile picture");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }


        [HttpGet("GetAllemployees")]
        public async Task<IActionResult> GetallEmployees()
        {
            return Ok(await _employeeService.GetEmpolyees());
        }
        [HttpPut("updateEmployeeWithProfilePic")]
        public async Task<IActionResult> UpdateEmployeeWithProfilePic(int employeeId, [FromForm] EmployeeData employee, IFormFile profilePic)
        {
            try
            {
                if (employee == null || profilePic == null || profilePic.Length == 0)
                {
                    return BadRequest("Invalid request data");
                }

                // Update the employee data
                var updatedEmployee = await _employeeService.UpdateEmployee(employeeId, employee);

                // Generate a unique blob name using the employee's ID and the file extension
                string blobName = $"{employeeId}{Path.GetExtension(profilePic.FileName)}";

                // Upload the new profile picture to Blob Storage with the generated blob name
                await _employeeService.UploadProfilePic(profilePic, blobName);

                if (updatedEmployee == null)
                {
                    return NotFound($"Employee with ID {employeeId} not found");
                }

                return Ok(updatedEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating data: {ex.Message}");
            }
        }


        //[HttpPut("{employeeId}")]
        //public async Task<IActionResult> UpdateEmployee(int employeeId, EmployeeData employee)
        //{
        //    try
        //    {
        //        var updatedEmployee = await _employeeService.UpdateEmployee(employeeId, employee);
        //        if (updatedEmployee == null)
        //        {
        //            return NotFound($"Employee with id = {employeeId} is not found");
        //        }

        //        return Ok(updatedEmployee);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating data: {ex.Message}");
        //    }
        //}

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
