using System.Security.Claims;

namespace SteadySchedule.Services
{
    public class UserContextService
    {
        public int? GetCompanyId(ClaimsPrincipal user)
        {
            var value = user.FindFirst("CompanyId")?.Value;

            return int.TryParse(value, out var companyId)
                ? companyId
                : null;
        }

        public bool TryInitializeCompany(ClaimsPrincipal user, ScheduleService schedule)
        {
            var companyId = GetCompanyId(user);

            if (companyId is not int id)
                return false;

            schedule.SetCompany(id);
            return true;
        }
    }
}