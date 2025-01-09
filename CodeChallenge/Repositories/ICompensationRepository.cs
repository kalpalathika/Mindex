using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface ICompensationRepository
    {
        Compensation Add(Compensation compensation);
        Task SaveAsync();
        Task<Compensation> GetByEmployeeIdAsync(string employeeId);
    }
}