using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        Task<Compensation> GetCompensationByEmployeeIdAsync(string employeeId);
        Task SaveAsync();
        Task<Compensation> CreateCompensationAsync(string employeeId, decimal salary, DateTime effectiveDate);

    }
}
