using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EmployeedataUsingSql.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace EmployeedataUsingSql.Data
{
    public class EmployeeService
    {
        private readonly EmployeeDbContext _employeeDbContext;

        private readonly BlobServiceClient _blobServiceClient;

        //private const string EmployeeImageContainer = "employeeimage";
        private readonly string _containerName;
        public EmployeeService(EmployeeDbContext employeeDbContext,IConfiguration configuration)
        {
            _employeeDbContext = employeeDbContext;

            var connectionString = configuration.GetConnectionString("BlobStorageConnection");
            _containerName = configuration.GetValue<string>("ConnectionStrings:ContainerName");
            _blobServiceClient = new BlobServiceClient(connectionString);

        }
        public async Task<string> UploadProfilePic(IFormFile ProfielPic, string ProfileName )
        {
            if (ProfielPic == null || ProfielPic.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            try
            {
                // Generate a unique blob name, or you can use the user's ID or another identifier
                //string uniqueBlobName = Guid.NewGuid().ToString();

                // Create a BlobContainerClient
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

                // Get a BlobClient for the new blob
                BlobClient blobClient = containerClient.GetBlobClient(ProfileName);

                // Upload the file to Blob Storage with content type
                await blobClient.UploadAsync(ProfielPic.OpenReadStream(), new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = ProfielPic.ContentType // Set the content type based on the file's MIME type
                    }
                });

                // Return the URL of the uploaded blob
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload profile picture: {ex.Message}");
            }
        }
        public async Task<Stream> GetProfilePicAsync(string employeeID)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(employeeID);

            try
            {
                var response = await blobClient.DownloadAsync();
                if (response != null)
                {
                    return response.Value.Content;
                }
            }
            catch (RequestFailedException ex)
            {
                // Handle the case where the blob is not found (404 error)
                if (ex.Status == 404)
                {
                    // Return null to indicate that the blob was not found
                    return null;
                }

                // Handle other exceptions if necessary
                // You can log the error, throw a custom exception, or take other actions
                // ...
            }

            // Return null if any exceptions occur
            return null;
        }

        //public async Task<Stream> DownloadFileAsync(string employeeID)
        //{
        //    var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        //    var blobClient = containerClient.GetBlobClient(employeeID);
        //    var response = await blobClient.DownloadAsync();
        //    return response.Value.Content;
        //}


        public async Task<EmployeeData> AddEmployee(EmployeeData employee)
        {
            var result = await _employeeDbContext.EmployeesData.AddAsync(employee);
            await _employeeDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<EmployeeData>GetEmployeeData(int employeeID)
        {
            return await _employeeDbContext.EmployeesData.FirstOrDefaultAsync(e => e.employeeId == employeeID);

        }
        public async Task<EmployeeData> DeleteEmployee(int employeeId)
        {
            var result = await GetEmployeeData(employeeId);
            if(result != null)
            {
                _employeeDbContext.EmployeesData.Remove(result);
                await _employeeDbContext.SaveChangesAsync();
                return result;

            }
            return null;
     
        }
        public async Task<bool> DeleteProfilePic(string employeeId)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            var blobClient = containerClient.GetBlobClient(employeeId);

            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteAsync();
                return true;
            }

            return false;

        }
        public async Task<IEnumerable<EmployeeData>> GetEmpolyees()
        {
            return await _employeeDbContext.EmployeesData.ToListAsync();
        }
        //public async Task<List<Stream>> GetAllProfilePicturesAsync()
        //{
        //    List<Stream> profilePictures = new List<Stream>();

        //    var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        //    // List all blobs in the container
        //    await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        //    {
        //        BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);

        //        // Download the blob content as a stream
        //        BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

        //        if (blobDownloadInfo != null)
        //        {
        //            profilePictures.Add(blobDownloadInfo.Content);
                 
        //        }
        //    }

        //    return profilePictures;
        //}
        public async Task<EmployeeData> UpdateEmployee(int employeeId, EmployeeData employeeData)
        {
            try
            {
                var existingEmployee = await _employeeDbContext.EmployeesData
                    .FirstOrDefaultAsync(e => e.employeeId == employeeId);

                if (existingEmployee != null)
                {
                    // Update the properties of the existing employee
                    existingEmployee.firstName = employeeData.firstName;
                    existingEmployee.lastName = employeeData.lastName;
                    existingEmployee.location = employeeData.location;
                    existingEmployee.department = employeeData.department;
                    existingEmployee.desigination = employeeData.desigination;
                    existingEmployee.skill = employeeData.skill;
                    existingEmployee.salary = employeeData.salary;

                    // Save changes to the database
                    await _employeeDbContext.SaveChangesAsync();

                    return existingEmployee; // Return the updated employee
                }

                return null; // Employee not found
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<EmployeeData>> Search(string name,int? employeeId)
        {
            IQueryable<EmployeeData> query = _employeeDbContext.EmployeesData;
            if (!string.IsNullOrEmpty(name))
            {
                query=query.Where(e => e.firstName.Contains(name) || e.lastName.Contains(name));
            }
            if(employeeId != null)
            {
                query=query.Where(e => e.employeeId == employeeId);
            }
            return await query.ToListAsync();
        }
    }
}
