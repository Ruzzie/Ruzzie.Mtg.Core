using System;

namespace Ruzzie.Mtg.Core
{
    /// <summary>
    /// Represents a basic type for an Mtg Card. This is a flags enum so multiple types can be combined.
    /// </summary>
    [Flags]
    public enum BasicType
    {
        /// <summary>
        /// Default: No type.
        /// </summary>
        None = 0,
        /// <summary>
        /// Artifact
        /// </summary>
        Artifact = 2,
        /// <summary>
        /// Creature
        /// </summary>
        Creature = 4,
        /// <summary>
        /// Enchantment
        /// </summary>
        Enchantment = 8,
        /// <summary>
        /// Instant
        /// </summary>
        Instant = 16,
        /// <summary>
        /// Sorcery
        /// </summary>
        Sorcery = 32,
        /// <summary>
        /// Basic land
        /// </summary>
        BasicLand = 64,
        /// <summary>
        /// Land, or non basic land
        /// </summary>
        Land = 128,
        /// <summary>
        /// Planeswalker
        /// </summary>
        Planeswalker = 256    
    }
}