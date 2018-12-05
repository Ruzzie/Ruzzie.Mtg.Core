using System;

namespace Ruzzie.Mtg.Core.Synergy
{
    /// <summary></summary>
    /// <typeparam name="T"></typeparam>
    public interface IValueConstrainable<in T> where T: struct, IEquatable<T>, IComparable<T>
    {
        /// <summary>Gets the value constraint.</summary>
        /// <returns></returns>
        IValueConstraint<T> GetValueConstraint();
    }
}