using Microsoft.AspNetCore.Mvc.RazorPages;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace MansorySupplyHub.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        private readonly INotyfService _notyf;

        public AccessDeniedModel(INotyfService notyf)
        {
            _notyf = notyf;
        }

        public void OnGet()
        {
            // Notify user about access denial
            _notyf.Error("Access denied. You do not have permission to view this page.");
        }
    }
}
