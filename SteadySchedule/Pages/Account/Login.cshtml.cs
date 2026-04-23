using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SteadySchedule.Data;
using System.Security.Claims;

namespace SteadySchedule.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
{
    var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);

    if (user == null)
    {
        ErrorMessage = "User not found.";
        return Page();
    }

    var claims = await _signInManager.UserManager.GetClaimsAsync(user);

    var isEmployee = claims.Any(c => c.Type == "Role" && c.Value == "Employee");
    var isAdmin = claims.Any(c => c.Type == "Role" && c.Value == "Admin");

    if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        return LocalRedirect(ReturnUrl);

    if (isAdmin)
        return LocalRedirect("/dashboard");

    if (isEmployee)
        return LocalRedirect("/myschedule");

    // fallback (just in case)
    return LocalRedirect("/dashboard");
}
}
    }
}