﻿using Veyesys.Core.Domain.Localization;

namespace Veyesys.Core.Domain.Common
{
    /// <summary>
    /// Represents an address attribute
    /// </summary>
    public partial class AddressAttribute : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

    }
}
