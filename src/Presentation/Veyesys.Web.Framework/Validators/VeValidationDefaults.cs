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


namespace Veyesys.Web.Framework.Validators
{
    /// <summary>
    /// Represents default values related to validation
    /// </summary>
    public static partial class VeValidationDefaults
    {
        /// <summary>
        /// Gets the name of a rule set used to validate model
        /// </summary>
        public static string ValidationRuleSet => "Validate";

        /// <summary>
        /// Gets the name of a locale used in not-null validation
        /// </summary>
        public static string NotNullValidationLocaleName => "Admin.Common.Validation.NotEmpty";
    }
}