using Hangfire.Dashboard;

namespace Dactra
{
    public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
