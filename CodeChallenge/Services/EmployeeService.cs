using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }
        
        /// <summary>
        /// Gets the reporting structure for a given employee.
        /// </summary>
        /// <param name="employeeId">The unique identifier of the employee.</param>
        /// <returns>The reporting structure of the employee including their direct reports and total number of reports.</returns>
        /// <exception cref="ArgumentException">Thrown when the employee with the provided ID is not found.</exception>
        public async Task<ReportingStructure> GetReportingStructure(string employeeId){
            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (employee == null){
                throw new ArgumentException($"Employee with {employeeId} not found.");
            }

            int totalReports = await CalculateTotalReports(employee);
            return new ReportingStructure(employee, totalReports);
        }

        /// <summary>
        /// Calculates the total number of reports for the given employee, including indirect reports.
        /// </summary>
        /// <param name="employee">The employee for whom to calculate the total reports.</param>
        /// <returns>The total number of reports (direct and indirect).</returns>

        private async Task<int> CalculateTotalReports(Employee employee)
        {
            if (employee.DirectReports == null || employee.DirectReports.Count == 0)
            {
                return 0;
            }

            int totalReports = employee.DirectReports.Count;

            foreach (var directReportId in employee.DirectReports){
                var directReport = await _employeeRepository.GetByIdAsync(directReportId.EmployeeId);
                if (directReport != null){
                    totalReports += await CalculateTotalReports(directReport);
                }
            }

            return totalReports;
        }

    }
}
