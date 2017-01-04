using System;

namespace Ruzzie.Mtg.Core
{
    [Flags]
    public enum Format
    {
        None = 0,
        Standard = 2,
        Modern = 4,
        Legacy = 8,
        Vintage = 16,
        Pauper = 32,
        Penny = 64,
        Duel = 128,
        Commander = 256,
        Frontier = 512
    }
}