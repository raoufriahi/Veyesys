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


namespace Veyesys.Web.Framework.Mvc.Routing
{
    public static partial class VePathRouteDefaults
    {
        /// <summary>
        /// Gets default key for action field
        /// </summary>
        public static string ActionFieldKey => "action";

        /// <summary>
        /// Gets default key for controller field
        /// </summary>
        public static string ControllerFieldKey => "controller";

        /// <summary>
        /// Gets default key for permanent redirect field
        /// </summary>
        public static string PermanentRedirectFieldKey => "permanentRedirect";

        /// <summary>
        /// Gets default key for url field
        /// </summary>
        public static string UrlFieldKey => "url";

        /// <summary>
        /// Gets default key for blogpost id field
        /// </summary>
        public static string BlogPostIdFieldKey => "blogpostId";

        /// <summary>
        /// Gets default key for category id field
        /// </summary>
        public static string CategoryIdFieldKey => "categoryid";

        /// <summary>
        /// Gets default key for manufacturer id field
        /// </summary>
        public static string ManufacturerIdFieldKey => "manufacturerid";

        /// <summary>
        /// Gets default key for newsitem id field
        /// </summary>
        public static string NewsItemIdFieldKey => "newsitemId";

        /// <summary>
        /// Gets default key for product id field
        /// </summary>
        public static string ProductIdFieldKey => "productid";

        /// <summary>
        /// Gets default key for product tag id field
        /// </summary>
        public static string ProducttagIdFieldKey => "productTagId";

        /// <summary>
        /// Gets default key for se name field
        /// </summary>
        public static string SeNameFieldKey => "sename";

        /// <summary>
        /// Gets default key for topic id field
        /// </summary>
        public static string TopicIdFieldKey => "topicid";

        /// <summary>
        /// Gets default key for vendor id field
        /// </summary>
        public static string VendorIdFieldKey => "vendorid";

        /// <summary>
        /// Gets language route value
        /// </summary>
        public static string LanguageRouteValue => "language";

        /// <summary>
        /// Gets language parameter transformer
        /// </summary>
        public static string LanguageParameterTransformer => "lang";
    }
}