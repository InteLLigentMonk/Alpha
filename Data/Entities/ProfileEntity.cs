using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProfileEntity
{
    [Key]
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StreetAddress { get; set; }
    public string? StreetNumber { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }

    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

}
