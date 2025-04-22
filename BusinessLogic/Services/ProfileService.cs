using System.Diagnostics;
using BusinessLogic.Factories;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace BusinessLogic.Services;

public class ProfileService(IProfileRepository profileRepository) : IProfileService
{
    private readonly IProfileRepository _profileRepository = profileRepository;

    public async Task<ServiceResult<Profile>> GetProfile(Guid userId)
    {
        if (userId != Guid.Empty)
        {
            var exists = await _profileRepository.Exsists(u => u.UserId == userId);
            if (exists.Result)
            {
                var response = await _profileRepository.GetAsync(u => u.UserId == userId, x => x.JobTitle);
                if (response.Result != null)
                {
                    return new ServiceResult<Profile>
                    {
                        Succeeded = true,
                        StatusCode = 200,
                        Result = response.Result
                    };
                }
            }
        }
        return new ServiceResult<Profile>
        {
            Succeeded = false,
            StatusCode = 404,
            Error = "Profile not found"
        };
    }

    public async Task<ServiceResult<bool>> CreateProfile(Profile profile)
    {
        if (profile != null)
        {
            var entity = ProfileFactory.Profile(profile);
            var exists = await _profileRepository.Exsists(p => p.UserId == profile.UserId);
            if (!exists.Result)
            {
                await _profileRepository.BeginTransactionAsync();
                try
                {
                    await _profileRepository.CreateAsync(entity);
                    await _profileRepository.SaveAsync();
                    await _profileRepository.CommitTransactionAsync();
                    return new ServiceResult<bool>
                    {
                        Succeeded = true,
                        StatusCode = 200,
                        Result = true
                    };
                }
                catch (Exception ex)
                {
                    await _profileRepository.RollbackTransactionAsync();
                    Debug.WriteLine(ex.Message);
                    return new ServiceResult<bool>
                    {
                        Succeeded = false,
                        StatusCode = 500,
                        Error = "Could not create profile",
                        Result = false
                    };
                }
            }
        }
        return new ServiceResult<bool>
        {
            Succeeded = false,
            StatusCode = 400,
            Error = "Profile can't be null"
        };
    }

    public async Task<ServiceResult<bool>> UpdateProfile(Profile profile)
    {
        if (profile != null)
        {
            var entity = ProfileFactory.Profile(profile);
            var exists = await _profileRepository.Exsists(p => p.Id == profile.Id);
            if (exists.Result)
            {
                await _profileRepository.BeginTransactionAsync();
                try
                {
                    _profileRepository.Update(entity);
                    await _profileRepository.SaveAsync();
                    await _profileRepository.CommitTransactionAsync();
                    return new ServiceResult<bool>
                    {
                        Succeeded = true,
                        StatusCode = 200,
                        Result = true
                    };
                }
                catch (Exception ex)
                {
                    await _profileRepository.RollbackTransactionAsync();
                    Debug.WriteLine(ex.Message);
                    return new ServiceResult<bool>
                    {
                        Succeeded = false,
                        StatusCode = 500,
                        Error = "Could not update profile",
                        Result = false
                    };
                }
            }
            return new ServiceResult<bool>
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "Profile not found"
            };
        }
        return new ServiceResult<bool>
        {
            Succeeded = false,
            StatusCode = 400,
            Error = "Profile can't be null"
        };
    }
}
