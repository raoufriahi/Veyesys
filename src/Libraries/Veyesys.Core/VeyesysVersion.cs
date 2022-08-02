namespace Veyesys.Core
{
    /// <summary>
    /// Represents nopCommerce version
    /// </summary>
    public static class VeyesysVersion
    {
        /// <summary>
        /// Gets the major store version
        /// </summary>
        public const string CURRENT_VERSION = "4.50";

        /// <summary>
        /// Gets the minor store version
        /// </summary>
        public const string MINOR_VERSION = "3";
        
        /// <summary>
        /// Gets the full store version
        /// </summary>
        public const string FULL_VERSION = CURRENT_VERSION + "." + MINOR_VERSION;
    }
}