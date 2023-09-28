using EmployeedataUsingSql.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeedataUsingSql.Data
{
    public class EmployeeService
    {
        private readonly EmployeeDbContext _employeeDbContext;
        public EmployeeService(EmployeeDbContext employeeDbContext)
        {
            _employeeDbContext = employeeDbContext;
        }
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
        public async Task<IEnumerable<EmployeeData>> GetEmpolyees()
        {
            return await _employeeDbContext.EmployeesData.ToListAsync();
        }
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
