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

using System.Collections.Generic;
using Veyesys.Core.Configuration;

namespace Veyesys.Core.Domain.Customers
{
    /// <summary>
    /// External authentication settings
    /// </summary>
    public class ExternalAuthenticationSettings : ISettings
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalAuthenticationSettings()
        {
            ActiveAuthenticationMethodSystemNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether email validation is required.
        /// In most cases we can skip email validation for Facebook or any other third-party external authentication plugins. I guess we can trust  Facebook for the validation.
        /// </summary>
        public bool RequireEmailValidation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need to logging errors on authentication process 
        /// </summary>
        public bool LogErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to remove external authentication associations
        /// </summary>
        public bool AllowCustomersToRemoveAssociations { get; set; }

        /// <summary>
        /// Gets or sets system names of active authentication methods
        /// </summary>
        public List<string> ActiveAuthenticationMethodSystemNames { get; set; }
    }
}
