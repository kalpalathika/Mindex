using System;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace challenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;

        public CompensationRepository(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        public Compensation Add(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();
            _employeeContext.Compensations.Add(compensation);
            return compensation;
        }

       public async Task<Compensation> GetByEmployeeIdAsync(string employeeId)
        {
            var compensation = await _employeeContext.Compensations
                .Include(c => c.Employee)
                .SingleOrDefaultAsync(c => c.Employee.EmployeeId == employeeId);

            if (compensation != null)
            {
                Console.WriteLine($"Compensation for Employee {compensation.Employee.EmployeeId} retrieved successfully.");
            }
            else
            {
                Console.WriteLine($"No compensation found for Employee with ID {employeeId}.");
            }

            return compensation;
        }

        
        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

    }
}
