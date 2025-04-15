using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Interfaces
{
    public interface IAuthService
    {
        Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
    }
}