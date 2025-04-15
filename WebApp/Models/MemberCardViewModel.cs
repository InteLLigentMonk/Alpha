using Domain.Models;

namespace WebApp.Models
{
    public class MemberCardViewModel
    {
        public NewMemberFormViewModel Form { get; set; } = null!;

        public Member Member { get; set; } = null!;
    }
}
