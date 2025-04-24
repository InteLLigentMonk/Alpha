using Data.Entities;
using Domain.Models;

namespace BusinessLogic.Factories;

public class ProfileFactory
{
    public static ProfileEntity NewProfileEntity()
    {
        var entity = new ProfileEntity()
        {
            Id = Guid.NewGuid(),
        };

        return entity;
    }

    public static Profile NewProfile()
    {
        var profile = new Profile()
        {
            Id = Guid.NewGuid(),
            JobTitle = new JobTitle { Id = 9 }
        };

        return profile;
    }

    public static ProfileEntity Profile(Profile model)
    {
        var profile = new ProfileEntity()
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            StreetAddress = model.StreetAddress,
            StreetNumber = model.StreetNumber,
            ZipCode = model.ZipCode,
            City = model.City,
            Country = model.Country,
            DateOfBirth = model.DateOfBirth,
            AvatarUrl = model.AvatarUrl,
        };
        if (model.UserId.HasValue)
        {
            profile.UserId = model.UserId.Value;
        }
        if (model.JobTitle != null)
        {
            profile.JobTitleId = model.JobTitle.Id;
        }
        return profile;
    }
}
