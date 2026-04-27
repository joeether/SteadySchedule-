using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SteadySchedule.Data;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace SteadySchedule.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty(SupportsGet = true)]
        public string? Email { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Token { get; set; }

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
            Input.Email = Email ?? "";
            Input.Token = Token ?? "";
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? Message { get; set; }

        public class InputModel
        {
            [Required]
            public string Email { get; set; } = "";

            [Required]
            public string Token { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [Required]
            [Compare("Password")]
            public string ConfirmPassword { get; set; } = "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                Message = "User not found.";
                return Page();
            }

            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(Input.Token));

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                Input.Password
            );

            if (result.Succeeded)
            {
                return RedirectToPage("/Account/Login");
            }

            Message = string.Join(", ", result.Errors.Select(e => e.Description));
            return Page();
        }
    }
}