using BusinessLogic.Models;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IJobTitleService
    {
        Task<ServiceResult<bool>> CreateJobTitle(JobTitleRegistrationFrom jobTitle);
        Task<ServiceResult<List<JobTitle>>> GetAllJobTitles();
        Task<ServiceResult<JobTitle>> GetJobTitle(int id);
    }
}