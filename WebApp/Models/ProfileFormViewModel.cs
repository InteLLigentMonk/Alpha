using System.ComponentModel.DataAnnotations;
using Domain.Models;

namespace WebApp.Models;

public class ProfileFormViewModel
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    [Display(Name = "First Name", Prompt ="Bruce")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name", Prompt ="Wayne")]
    public string? LastName { get; set; }

    [Display(Name = "Email", Prompt = "Email address")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Must be a valid Email")]
    [Required(ErrorMessage = "Required")]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number", Prompt = "Your phone number...")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Job Title", Prompt = "Batman")]
    public string? JobTitle { get; set; }

    [Display(Name = "Street Name", Prompt ="Street Name")]
    public string? StreetAddress { get; set; }

    [Display(Name = "Number", Prompt = "Number")]
    public string? StreetNumber { get; set; }

    [Display(Name = "Zip Code", Prompt = "Zip Code")]
    public string? ZipCode { get; set; }

    [Display(Name = "City", Prompt = "City")]
    public string? City { get; set; }

    [Display(Name = "Country", Prompt = "Country")]
    public string? Country { get; set; }

    [Display(Name = "Day")]
    public int Day { get; set; }

    [Display(Name = "Month")]
    public int Month { get; set; }

    [Display(Name = "Year")]
    public int Year { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth
    {
        get
        {
            try
            {
                return new DateTime(Year, Month, Day);
            }
            catch
            {
                return null;
            }
        }
    }

    public string? AvatarUrl { get; set; }

    public IFormFile? Avatar { get; set; }

    public static implicit operator Profile(ProfileFormViewModel model)
    {
        var profile = new Profile
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            JobTitle = model.JobTitle,
            StreetAddress = model.StreetAddress,
            StreetNumber = model.StreetNumber,
            ZipCode = model.ZipCode,
            City = model.City,
            Country = model.Country,
            DateOfBirth = new DateTime(model.Year, model.Month, model.Day),
            AvatarUrl = model.AvatarUrl
        };

        if (model.UserId.HasValue)
        {
            profile.UserId = model.UserId;
        }
        return profile;
    }
}
