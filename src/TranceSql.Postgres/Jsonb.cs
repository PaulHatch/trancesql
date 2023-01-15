using System;

namespace TranceSql.Postgres
{
    /// <summary>
    /// Defines helper methods for creating Postgres jsonb function declarations.
    /// </summary>
    public static class Jsonb
    {
        /// <summary>
        /// Creates a jsonb_set function which update the target at the
        /// specified path with the new value provided, returning the
        /// updated target as the result.
        /// </summary>
        /// <param name="target">Value to update</param>
        /// <param name="path">Path to set.</param>
        /// <param name="newValue">New value to replace at path.</param>
        /// <param name="createMissing">Boolean value indicating whether
        /// the value should be created if missing.</param>
        /// <returns>Function call with specified parameters.</returns>
        public static Function JsonbSet(ISqlElement target, JsonPath path, ISqlElement newValue, ISqlElement createMissing)
            => new("jsonb_set", target, Constant.Unsafe($"{{{path}}}"), newValue, createMissing);

        /// <summary>
        /// Creates a jsonb_set function which update the target at the
        /// specified path with the new value provided, returning the
        /// updated target as the result.
        /// </summary>
        /// <param name="target">Value to update</param>
        /// <param name="path">Path to set.</param>
        /// <param name="newValue">New value to replace at path.</param>
        /// <param name="createMissing">Boolean value indicating whether
        /// the value should be created if missing.</param>
        /// <returns>Function call with specified parameters.</returns>
        public static Function JsonbSet(ISqlElement target, JsonPath path, ISqlElement newValue, bool createMissing)
            => JsonbSet(target, string.Join(",", path), newValue, new Constant(createMissing));
    }
}
