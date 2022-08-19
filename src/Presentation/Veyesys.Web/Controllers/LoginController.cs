using Microsoft.AspNetCore.Mvc;
namespace Veyesys.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class LoginController : Controller
    {
 
        // GET: LoginController
        public ActionResult Login()
        {
            return View();
        }
    }
}
