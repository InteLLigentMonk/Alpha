using Data.Entities;
using Data.Interfaces;

namespace BusinessLogic.Services;

public class ProfileService(IProfileRepository profileRepository)
{
    private readonly IProfileRepository _profileRepository = profileRepository;

    public async Task<ProfileEntity?> GetProfile(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty", nameof(userId));
        }

        if (await _profileRepository.Exsists(u => u.UserId == userId))
        {
            return await _profileRepository.GetAsync(u => u.UserId == userId);
        }
        return null;
    }

    public async Task<bool> CreateProfile(ProfileEntity profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        if (await _profileRepository.Exsists(p => p.UserId == profile.UserId))
        {
            throw new ArgumentException("Profile already exists", nameof(profile));
        }

        await _profileRepository.BeginTransactionAsync();
        try
        {
            await _profileRepository.CreateAsync(profile);
            await _profileRepository.SaveAsync();
            await _profileRepository.CommitTransactionAsync();
            return true;
        }
        catch (Exception ex)
        {
            await _profileRepository.RollbackTransactionAsync();
            throw new Exception("Could not create profile", ex);
        }

    }

    public async Task<bool> UpdateProfile(ProfileEntity profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        if (!await _profileRepository.Exsists(p => p.UserId == profile.UserId))
        {
            throw new ArgumentException("Profile does not exist", nameof(profile));
        }
        await _profileRepository.BeginTransactionAsync();
        try
        {
            _profileRepository.Update(profile);
            await _profileRepository.SaveAsync();
            await _profileRepository.CommitTransactionAsync();
            return true;
        }
        catch (Exception ex)
        {
            await _profileRepository.RollbackTransactionAsync();
            throw new Exception("Could not update profile", ex);
        }
    }
}
