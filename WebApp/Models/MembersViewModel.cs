using BusinessLogic.Models;

namespace WebApp.Models;

public class MembersViewModel
{
    public IEnumerable<Member> Members { get; set; } = [];

    public NewMemberFormViewModel NewMemberForm { get; set; } = new();
}
