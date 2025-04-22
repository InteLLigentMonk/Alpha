namespace Domain.Models;

public class Profile
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public JobTitle? JobTitle { get; set; }
    public string? StreetAddress { get; set; }
    public string? StreetNumber { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime DateOfBirth { get; set; } = DateTime.Today;
    public string? AvatarUrl { get; set; }
    public Guid? UserId { get; set; }
}
