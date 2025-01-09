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

        /// <summary>
        /// Retrieves the latest compensation record for a specified employee based on the effective date.
        /// </summary>
        /// <param name="employeeId">The unique identifier of the employee.</param>
        /// <returns>The most recent <see cref="Compensation"/> record, or null if none exist.</returns>

       public async Task<Compensation> GetByEmployeeIdAsync(string employeeId)
        {
            var compensation = await _employeeContext.Compensations
                    .Include(c => c.Employee)
                    .Where(c => c.Employee.EmployeeId == employeeId)
                    .OrderByDescending(c => c.EffectiveDate)
                    .FirstOrDefaultAsync();

            return compensation;
        }

        
        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

    }
}
