using Microsoft.AspNetCore.Identity;

namespace SteadySchedule.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int? CompanyId { get; set; }
    }
}