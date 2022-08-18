using Veyesys.Core.Domain.Localization;
using Veyesys.Core.Infrastructure;
using Veyesys.Data;
using Veyesys.Web.Framework.Mvc.Routing;

namespace Veyesys.Web.Infrastructure
{
    /// <summary>
    /// Represents base provider
    /// </summary>
    public class BaseRouteProvider
    {
        /// <summary>
        /// Get pattern used to detect routes with language code
        /// </summary>
        /// <returns></returns>
        protected string GetLanguageRoutePattern()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //this pattern is set once at the application start, when we don't have the selected language yet
                    //so we use 'en' by default for the language value, later it'll be replaced with the working language code
                    var code = "en";
                    return $"{{{VePathRouteDefaults.LanguageRouteValue}:maxlength(2):{VePathRouteDefaults.LanguageParameterTransformer}={code}}}";
                }
            }

            return string.Empty;
        }
    }
}