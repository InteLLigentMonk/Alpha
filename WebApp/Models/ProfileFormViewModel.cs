using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class ProfileFormViewModel
{
    [Display(Name = "First Name", Prompt ="Bruce")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name", Prompt ="Wayne")]
    public string? LastName { get; set; }
    
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
}
