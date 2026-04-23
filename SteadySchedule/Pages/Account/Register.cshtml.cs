using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SteadySchedule.Data;
using SteadySchedule.Domain;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SteadySchedule.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public RegisterModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            public string CompanyName { get; set; } = "";

            [Required]
            [EmailAddress]
            public string Email { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; } = "";
        }

        public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
        return Page();

    var user = new ApplicationUser
    {
        UserName = Input.Email,
        Email = Input.Email
    };

    var result = await _userManager.CreateAsync(user, Input.Password);

    if (!result.Succeeded)
    {
        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return Page();
    }

    var company = new Company
    {
        Name = Input.CompanyName.Trim()
    };

    _context.Companies.Add(company);
    await _context.SaveChangesAsync();

    // link user to THEIR company
    user.CompanyId = company.Id;
    await _userManager.UpdateAsync(user);

    // claims
    var companyClaimResult = await _userManager.AddClaimAsync(
        user,
        new Claim("CompanyId", company.Id.ToString()));

    var roleClaimResult = await _userManager.AddClaimAsync(
        user,
        new Claim("Role", "Admin"));

    if (!companyClaimResult.Succeeded || !roleClaimResult.Succeeded)
    {
        ModelState.AddModelError("", "Error setting up user account.");
        return Page();
    }

    await _signInManager.SignInAsync(user, isPersistent: false);

    return LocalRedirect(ReturnUrl ?? "/dashboard");
}
    }
}