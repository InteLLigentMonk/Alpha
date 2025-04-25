using BusinessLogic.Models;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IJobTitleService
    {
        Task<ServiceResult<bool>> CreateJobTitle(JobTitleRegistrationFrom jobTitle);
        Task<ServiceResult<bool>> Delete(int id);
        Task<ServiceResult<List<JobTitle>>> GetAllJobTitles();
        Task<ServiceResult<JobTitle>> GetJobTitle(int id);
    }
}