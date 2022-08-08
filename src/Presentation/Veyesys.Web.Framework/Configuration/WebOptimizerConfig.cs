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

using System.Text.Json.Serialization;
using Veyesys.Core.Configuration;
using WebOptimizer;

namespace Veyesys.Web.Framework.Configuration
{
    public class WebOptimizerConfig : WebOptimizerOptions, IConfig
    {
        #region Ctor

        public WebOptimizerConfig()
        {
            EnableDiskCache = true;
            EnableTagHelperBundling = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// A value indicating whether JS file bundling and minification is enabled
        /// </summary>
        public bool EnableJavaScriptBundling { get; private set; } = true;

        /// <summary>
        /// A value indicating whether CSS file bundling and minification is enabled
        /// </summary>
        public bool EnableCssBundling { get; private set; } = true;

        /// <summary>
        /// Gets or sets a suffix for the js-file name of generated bundles
        /// </summary>
        public string JavaScriptBundleSuffix { get; private set; } = ".scripts";

        /// <summary>
        /// Gets or sets a suffix for the css-file name of generated bundles
        /// </summary>
        public string CssBundleSuffix { get; private set; } = ".styles";

        /// <summary>
        /// Gets a section name to load configuration
        /// </summary>
        [JsonIgnore]
        public string Name => "WebOptimizer";

        /// <summary>
        /// Gets an order of configuration
        /// </summary>
        /// <returns>Order</returns>
        public int GetOrder() => 2;

        #endregion
    }
}