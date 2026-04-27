using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SteadySchedule.Data;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace SteadySchedule.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string Email { get; set; } = "";

        public string? ResetLink { get; set; }

        public async Task OnPostAsync()
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
                return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

            // BUILD FULL LINK
            ResetLink = $"/Account/ResetPassword?email={Email}&token={encodedToken}";
        }
    }
}