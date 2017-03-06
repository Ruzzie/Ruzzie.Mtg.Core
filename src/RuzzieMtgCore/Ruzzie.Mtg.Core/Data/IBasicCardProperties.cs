namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Interface for minimum properties a Mtg card has.
    /// </summary>
    public interface IBasicCardProperties : IHasName
    {
        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        double Price { get; set; }
        /// <summary>
        /// Gets or sets the CMC. (converted mana costs)
        /// </summary>
        /// <value>
        /// The CMC.
        /// </value>
        int Cmc { get; set; }
        /// <summary>
        /// Gets or sets the color identity. (This is an integer representation of the <see cref="ColorIdentity"/> enum.)
        /// </summary>
        /// <value>
        /// The color identity.
        /// </value>
        int ColorIdentity { get; set; }
        /// <summary>
        /// Gets or sets the Basic card type. (This is an integer representation of the <see cref="BasicType"/> enum.)
        /// </summary>
        /// <value>
        /// The type of the basic.
        /// </value>
        int BasicType { get; set; }
        /// <summary>
        /// All types as a string delimited by the - (space dash space) characters.
        /// </summary>
        /// <value>
        /// The types.
        /// </value>
        string Types { get; set; }
        /// <summary>
        /// Gets or sets the rating.
        /// </summary>
        /// <value>
        /// The rating.
        /// </value>
        double? Rating { get; set; }
        /// <summary>
        /// Gets or sets the Legal formats. (This is an integer representation of the <see cref="Format"/> enum.)
        /// </summary>
        /// <value>
        /// The legality.
        /// </value>
        int Legality { get; set; }
        /// <summary>
        /// Gets or sets the mana cost string (ex. {1}{W}{U}) or {W/U} or {2/B} or {U/P}.
        /// </summary>
        /// <value>
        /// The mana cost.
        /// </value>
        string ManaCost { get; set; }
    }
}