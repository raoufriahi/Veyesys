/*
 * Copyright (c) 2022-2023 Veyesys
 *
 * The computer program contained herein contains proprietary
 * information which is the property of Veyesys.
 * The program may be used and/or copied only with the written
 * permission of Veyesys or in accordance with the
 * terms and conditions stipulated in the agreement/contract under
 * which the programs have been supplied.
 */

using System.ComponentModel;
using Veyesys.Core;
using Veyesys.Core.Infrastructure;
using Veyesys.Services.Localization;

namespace Veyesys.Web.Framework.Mvc.ModelBinding
{
    /// <summary>
    /// Represents model attribute that specifies the display name by passed key of the locale resource
    /// </summary>
    public sealed class VeResourceDisplayNameAttribute : DisplayNameAttribute, IModelAttribute
    {
        #region Fields

        private string _resourceValue = string.Empty;

        #endregion

        #region Ctor

        /// <summary>
        /// Create instance of the attribute
        /// </summary>
        /// <param name="resourceKey">Key of the locale resource</param>
        public VeResourceDisplayNameAttribute(string resourceKey) : base(resourceKey)
        {
            ResourceKey = resourceKey;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets key of the locale resource 
        /// </summary>
        public string ResourceKey { get; set; }

        /// <summary>
        /// Gets the display name
        /// </summary>
        public override string DisplayName
        {
            get
            {
                //get working language identifier
                var workingLanguageId = EngineContext.Current.Resolve<IWorkContext>().GetWorkingLanguageAsync().Result.Id;

                //get locale resource value
                _resourceValue = EngineContext.Current.Resolve<ILocalizationService>().GetResourceAsync(ResourceKey, workingLanguageId, true, ResourceKey).Result;

                return _resourceValue;
            }
        }

        /// <summary>
        /// Gets name of the attribute
        /// </summary>
        public string Name => nameof(VeResourceDisplayNameAttribute);

        #endregion
    }
}
