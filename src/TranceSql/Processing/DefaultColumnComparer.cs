using System;
using System.Collections.Generic;

namespace TranceSql.Processing
{
    /// <summary>
    /// Default string comparison implementation for column names, this comparer
    /// is very permissive and ignores case differences and underscores to handle
    /// differences from Pascal case to camel case or snake case.
    /// </summary>
    internal class DefaultCaseComparer : IEqualityComparer<string>
    {
        /// <summary>
        /// Gets the default instance of the comparer.
        /// </summary>
        public static DefaultCaseComparer Comparer { get; } = new();

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(string? x, string? y)
        {

            if (x == null) { return string.IsNullOrEmpty(y); }
            if (y == null) { return string.IsNullOrEmpty(x); }

            int ix = 0, iy = 0;
            for (; ix < x.Length || iy < x.Length; ix++, iy++)
            {
                // Skip underscores
                for (; ix < x.Length && x[ix] == '_'; ix++) { }
                for (; iy < y.Length && y[iy] == '_'; iy++) { }

                // if while skipping we reached the end of either string, stop
                if (ix == x.Length) { return iy == y.Length; }
                if (iy == y.Length) { return ix == x.Length; }

                if (char.ToLowerInvariant(y[iy]) != char.ToLowerInvariant(x[ix]))
                {
                    return false;
                }
            }

            for (; ix < x.Length && x[ix] == '_'; ix++) { }
            for (; iy < y.Length && y[iy] == '_'; iy++) { }

            return ix == x.Length && iy == y.Length;
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(string obj)
            => obj
                .ToLowerInvariant()
                .Replace("_", "")
                .GetHashCode();
    }
}
