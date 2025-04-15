using Domain.Models;

namespace WebApp.Models;

public class MembersViewModel
{
    public IEnumerable<Member> Members { get; set; } = [];

    public ProfileFormViewModel NewProfileForm { get; set; } = new();
}
