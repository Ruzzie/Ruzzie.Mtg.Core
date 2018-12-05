using System;

namespace Ruzzie.Mtg.Core.Synergy
{
    /// <summary>TypeConstraints definitions, such that each T, TConstraint is a unique type and the Constraint is resolved.</summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TConstraint">The type of the constraint.</typeparam>
    internal static class TypeConstraints<T, TConstraint>  
        where T : struct, IEquatable<T>, IComparable<T>      
        where TConstraint : IValueConstraint<T>, new()
    {
        public static readonly TConstraint Constraint = new TConstraint();
    }
}