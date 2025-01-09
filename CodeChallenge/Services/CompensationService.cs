using challenge.Repositories;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly IEmployeeRepository _employeeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompensationService"/> class.
        /// </summary>
        /// <param name="compensationRepository">The compensation repository.</param>
        /// <param name="employeeRepository">The employee repository.</param>
        public CompensationService(ICompensationRepository compensationRepository, IEmployeeRepository employeeRepository)
        {
            _compensationRepository = compensationRepository;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Creates a new compensation record for an employee.
        /// </summary>
        /// <param name="employeeId">The employee's ID.</param>
        /// <param name="salary">The salary amount.</param>
        /// <param name="effectiveDate">The effective date for the compensation.</param>
        /// <returns>A task representing the asynchronous operation, with a <see cref="Compensation"/> result.</returns>
        /// <exception cref="ArgumentException">Thrown when the employee does not exist.</exception>
        public async Task<Compensation> CreateCompensationAsync(string employeeId, decimal salary, DateTime effectiveDate)
        {
            // Fetch the Employee object from the Employee DbSet
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                throw new ArgumentException($"Employee with ID {employeeId} does not exist.");
            }

            // Create a new Compensation object
            var compensation = new Compensation
            {
                CompensationId = Guid.NewGuid().ToString(),
                Employee = employee,
                Salary = salary,
                EffectiveDate = effectiveDate
            };

            // Add the Compensation to the database
            _compensationRepository.Add(compensation);
            await _compensationRepository.SaveAsync();

            return compensation;
        }


        public async Task<Compensation> GetCompensationByEmployeeIdAsync(string employeeId)
        {
            return await _compensationRepository.GetByEmployeeIdAsync(employeeId);
        }

        public async Task SaveAsync()
        {
            await _compensationRepository.SaveAsync();
        }
    }
}
