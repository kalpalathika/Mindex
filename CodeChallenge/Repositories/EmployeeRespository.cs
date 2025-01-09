using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
        }

        public async Task<Employee> GetByIdAsync(string id)
            {
                // Retrieve the employee asynchronously by their ID
                var employee = await _employeeContext.Employees
                    .FirstOrDefaultAsync(e => e.EmployeeId == id);

                if (employee != null)
                {
                    // Explicitly load the DirectReports collection for the employee
                    await _employeeContext.Entry(employee)
                        .Collection(e => e.DirectReports)
                        .LoadAsync();

                    Console.WriteLine($"Employee {employee.EmployeeId} has {employee.DirectReports.Count} direct reports.");
                }

                return employee;
            }


        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
