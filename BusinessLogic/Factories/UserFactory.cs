using Data.Entities;
using Domain.Models;

namespace BusinessLogic.Factories;

public class UserFactory
{
    public static Member Member() 
    { 
    return new Member();
    }

    public static Member Member(AppUser user)
    {
        return new Member
        {
            Id = user.Profile.Id,
            UserId = user.Id,
            FirstName = user.Profile.FirstName,
            LastName = user.Profile.LastName,
            PhoneNumber = user.Profile.PhoneNumber,
            EmailAddress = user.Email,
            StreetAddress = user.Profile.StreetAddress,
            StreetNumber = user.Profile.StreetNumber,
            ZipCode = user.Profile.ZipCode,
            City = user.Profile.City,
            Country = user.Profile.Country,
            JobTitle = user.Profile.JobTitle,
            DateOfBirth = user.Profile.DateOfBirth,
            AvatarUrl = user.Profile.AvatarUrl
        };
    }

}
