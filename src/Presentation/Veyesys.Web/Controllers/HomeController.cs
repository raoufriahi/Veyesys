using Microsoft.AspNetCore.Mvc;

namespace Veyesys.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        public virtual IActionResult Index()
        {

            return RedirectToRoute("Login");
        }

        public virtual IActionResult Home()
        {
            return View();
        }
    }
}