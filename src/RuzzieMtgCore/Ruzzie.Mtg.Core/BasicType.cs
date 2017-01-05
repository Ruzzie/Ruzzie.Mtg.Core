using System;

namespace Ruzzie.Mtg.Core
{
    [Flags]
    public enum BasicType
    {
        None = 0,
        Artifact = 2,
        Creature = 4,        
        Enchantment  = 8,
        Instant = 16,
        Sorcery = 32,
        BasicLand = 64,
        Land = 128,
        Planeswalker = 256    
    }
}