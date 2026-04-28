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
    public class JoinModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public JoinModel(
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

        [FromQuery(Name = "code")]
        [BindProperty(SupportsGet = true)]
        public string? InviteCode { get; set; }

        public class InputModel
        {
            [Required]
            public string InviteCode { get; set; } = "";

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

        public void OnGet()
        {
            if (!string.IsNullOrWhiteSpace(InviteCode))
            {
                Input.InviteCode = InviteCode;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // TEMP DEV invite code (still fine for now)
            var invite = await _context.InviteCodes
            .FirstOrDefaultAsync(i => i.Code == Input.InviteCode);

            if (invite == null)
            {
                ModelState.AddModelError(nameof(Input.InviteCode), "Invalid invite code.");
                return Page();
            }

            if (invite.IsUsed)
            {
                ModelState.AddModelError(nameof(Input.InviteCode), "This code has already been used.");
                return Page();
            }

            if (invite.ExpirationDate < DateTime.UtcNow)
            {
                ModelState.AddModelError(nameof(Input.InviteCode), "This code has expired.");
                return Page();
            }

            // GET employee FROM invite
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == invite.EmployeeId);

            if (employee == null)
            {
                ModelState.AddModelError("", "This invite is no longer valid.");
                return Page();
            }

            // VERIFY email matches invite
            if (!string.Equals(employee.Email, Input.Email, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "This invite does not match your email.");
                return Page();
            }

            var company = await _context.Companies
                .FirstAsync(c => c.Id == employee.CompanyId);

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

            invite.IsUsed = true;
            await _context.SaveChangesAsync();

            // link user to correct company
            user.CompanyId = company.Id;
            await _userManager.UpdateAsync(user);

            // claims (clean separation)
            await _userManager.AddClaimAsync(
                user,
                new Claim("CompanyId", company.Id.ToString()));

            await _userManager.AddClaimAsync(
                user,
                new Claim("EmployeeId", employee.Id.ToString()));

            await _userManager.AddClaimAsync(
                user,
                new Claim("Role", "Employee"));

            await _signInManager.SignInAsync(user, isPersistent: false);

            return LocalRedirect("/published");
        }
    }
}