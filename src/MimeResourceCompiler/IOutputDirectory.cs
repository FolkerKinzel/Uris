namespace MimeResourceCompiler
{
    /// <summary>
    /// Represents the output directory.
    /// </summary>
    public interface IOutputDirectory
    {
        /// <summary>
        /// Absolute path of the output directory.
        /// </summary>
        string FullName { get; }
    }
}