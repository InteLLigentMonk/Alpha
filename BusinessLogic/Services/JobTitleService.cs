using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace BusinessLogic.Services;

public class JobTitleService(IJobTitleRepository jobTitleRepository) : IJobTitleService
{
    private readonly IJobTitleRepository _jobTitleRepository = jobTitleRepository;




    public async Task<ServiceResult<JobTitle>> GetJobTitle(int id)
    {
        var exists = await _jobTitleRepository.Exsists(u => u.Id == id);
        if (exists.Result)
        {
            var response = await _jobTitleRepository.GetAsync(u => u.Id == id);
            if (response.Result != null)
            {
                return new ServiceResult<JobTitle>
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = response.Result
                };
            }
        }
        return new ServiceResult<JobTitle>
        {
            Succeeded = false,
            StatusCode = 404,
            Error = "Job title not found"
        };
    }

    public async Task<ServiceResult<List<JobTitle>>> GetAllJobTitles()
    {
        var response = await _jobTitleRepository.GetAllAsync();
        if (response.Succeeded && response.Result != null)
        {
            return new ServiceResult<List<JobTitle>>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = [.. response.Result]
            };
        }
        return new ServiceResult<List<JobTitle>>
        {
            Succeeded = false,
            StatusCode = 404,
            Error = "No job titles found"
        };
    }

    public async Task<ServiceResult<bool>> CreateJobTitle(JobTitleRegistrationFrom jobTitle)
    {
        if (jobTitle != null)
        {
            var entity = new JobTitleEntity
            {
                Title = jobTitle.Title
            };
            var exists = await _jobTitleRepository.Exsists(j => j.Title == jobTitle.Title);
            if (!exists.Succeeded)
            {
                await _jobTitleRepository.BeginTransactionAsync();
                try
                {
                    await _jobTitleRepository.CreateAsync(entity);
                    await _jobTitleRepository.SaveAsync();
                    await _jobTitleRepository.CommitTransactionAsync();
                    return new ServiceResult<bool>
                    {
                        Succeeded = true,
                        StatusCode = 200,
                        Result = true
                    };
                }
                catch (Exception ex)
                {
                    await _jobTitleRepository.RollbackTransactionAsync();
                    return new ServiceResult<bool>
                    {
                        Succeeded = false,
                        StatusCode = 500,
                        Error = ex.Message
                    };
                }
            }
            return new ServiceResult<bool>
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "Job title already exists"
            };
        }
        return new ServiceResult<bool>
        {
            Succeeded = false,
            StatusCode = 400,
            Error = "Job title is null"
        };
    }
}
