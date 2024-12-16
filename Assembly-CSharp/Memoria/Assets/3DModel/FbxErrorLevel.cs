namespace Memoria.Assets
{
    /// <summary>
    /// Indicates when a reader should throw errors
    /// </summary>
    public enum FbxErrorLevel
    {
        /// <summary>
        /// Ignores inconsistencies unless the parser can no longer continue
        /// </summary>
        Permissive = 0,

        /// <summary>
        /// Checks data integrity, such as checksums and end points
        /// </summary>
        Checked = 1,

        /// <summary>
        /// Checks everything, including magic bytes
        /// </summary>
        Strict = 2,
    }
}
