using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ProfileEntity
{
    [Key]
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StreetAddress { get; set; }
    public string? StreetNumber { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }

    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public int JobTitleId { get; set; }
    public JobTitleEntity? JobTitle { get; set; }

}
