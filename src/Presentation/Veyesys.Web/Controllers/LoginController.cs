using System;
using Microsoft.AspNetCore.Mvc;
using Veyesys.Web.Models.login;
using Veyesys.Web.Infrastructure.Installation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Veyesys.Core;
using Veyesys.Data;
namespace Veyesys.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class LoginController : Controller
    {
        #region Fields

        private readonly Lazy<IInstallationLocalizationService> _locService;

        #endregion

        #region Ctor
        public LoginController(Lazy<IInstallationLocalizationService> locService)
        {
            _locService = locService;
        }

        #endregion



        // GET: LoginController
        public ActionResult Login()
        {
            var model = new LoginViewModel
            {
                Username = "Raul",
                Password = "123456"  
            };
            // model = PrepareLanguageList(model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                return RedirectToRoute("Home");
            }
            return View();
        }


        private LoginViewModel PrepareLanguageList(LoginViewModel model)
        {
            foreach (var lang in _locService.Value.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Value = Url.Action("ChangeLanguage", "Login", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = _locService.Value.GetCurrentLanguage().Code == lang.Code
                });
            }
            return model;
        }
    }
}
