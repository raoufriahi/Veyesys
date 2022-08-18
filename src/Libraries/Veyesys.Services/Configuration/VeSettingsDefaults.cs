using Veyesys.Core.Caching;
using Veyesys.Core.Domain.Configuration;

namespace Veyesys.Services.Configuration
{
    /// <summary>
    /// Represents default values related to settings
    /// </summary>
    public static partial class VeSettingsDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey SettingsAllAsDictionaryCacheKey => new("Nop.setting.all.dictionary.", VeEntityCacheDefaults<Setting>.Prefix);

        #endregion
    }
}