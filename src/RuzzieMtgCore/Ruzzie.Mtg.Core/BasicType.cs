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
        /// Default: No type. (0)
        /// </summary>
        None = 0,
        //TODO: ADD 1
        /// <summary>
        /// Artifact, (2)
        /// </summary>
        Artifact = 2,
        /// <summary>
        /// Creature, (4)
        /// </summary>
        Creature = 4,
        /// <summary>
        /// Enchantment, (8)
        /// </summary>
        Enchantment = 8,
        /// <summary>
        /// Instant, (16)
        /// </summary>
        Instant = 16,
        /// <summary>
        /// Sorcery, (32)
        /// </summary>
        Sorcery = 32,
        /// <summary>
        /// Basic land, (64)
        /// </summary>
        BasicLand = 64,
        /// <summary>
        /// Land, (128)
        /// </summary>
        Land = 128,
        /// <summary>
        /// Planeswalker, (256)
        /// </summary>
        Planeswalker = 256    
    }
}