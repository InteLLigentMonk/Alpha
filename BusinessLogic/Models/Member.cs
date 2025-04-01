namespace BusinessLogic.Models;

public class Member
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? JobTitle { get; set; }
    public string? StreetAddress { get; set; }
    public string? StreetNumber { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? AvatarUrl { get; set; }
}
