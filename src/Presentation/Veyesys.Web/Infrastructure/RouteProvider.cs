using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Veyesys.Services.Installation;
using Veyesys.Web.Framework.Mvc.Routing;

namespace Veyesys.Web.Infrastructure
{
    /// <summary>
    /// Represents provider that provided basic routes
    /// </summary>
    public partial class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //get language pattern
            //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
            //use it only for URLs of pages that the user can go to
            var lang = GetLanguageRoutePattern();

            //areas
            endpointRouteBuilder.MapControllerRoute(name: "areaRoute",
                pattern: $"{{area:exists}}/{{controller=Home}}/{{action=Index}}/{{id?}}");

            //home page
            endpointRouteBuilder.MapControllerRoute(name: "Homepage", 
                pattern: $"{lang}",
                defaults: new { controller = "Home", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "Home",
             pattern: $"{lang}/Home",
             defaults: new { controller = "Home", action = "Home" });

            //change language
            endpointRouteBuilder.MapControllerRoute(name: "ChangeLanguage",
                pattern: $"{lang}/changelanguage/{{langid:min(0)}}",
                defaults: new { controller = "Common", action = "SetLanguage" });

            //page not found
            endpointRouteBuilder.MapControllerRoute(name: "PageNotFound",
                pattern: $"{lang}/page-not-found",
                defaults: new { controller = "Common", action = "PageNotFound" });

            //Login
            endpointRouteBuilder.MapControllerRoute(name: "Login",
                pattern: $"{lang}/Login",
                defaults: new { controller = "Login", action = "Login" });

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;

        #endregion
    }
}