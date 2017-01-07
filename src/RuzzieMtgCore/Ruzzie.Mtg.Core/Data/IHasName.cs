namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Interface that defines if a type has a Name property.
    /// </summary>
    public interface IHasName
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
    }
}