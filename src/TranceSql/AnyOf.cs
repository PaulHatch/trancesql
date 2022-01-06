namespace TranceSql
{
    /// <summary>
    /// Represents a value which can be one of multiple types all of which
    /// must be derived from the root type. This class supports implicit casting
    /// from the supported types.
    /// </summary>
    /// <typeparam name="T1">The first possible type.</typeparam>
    /// <typeparam name="T2">The second possible type.</typeparam>
    /// <typeparam name="TRoot">The type of the root.</typeparam>
    public class AnyOf<T1, T2, TRoot>
        where T1 : TRoot
        where T2 : TRoot
    {
        private AnyOf(TRoot value) => Value = value;

        /// <summary>
        /// Creates an instance using the root ancestor, used when interfaces
        /// are needed and implicit conversion cannot be used.
        /// </summary>
        /// <param name="value">The value to create.</param>
        /// <returns>New AnyOf instance.</returns>
        public static AnyOf<T1, T2, TRoot> Of(TRoot value) => new(value);

        /// <summary>
        /// Gets the value for this instance.
        /// </summary>
        public TRoot Value { get; }

        /// <summary>
        /// Performs an implicit conversion from T1 to <see cref="AnyOf{T1, T2, TRoot}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AnyOf<T1, T2, TRoot>(T1 value)
            => new(value);

        /// <summary>
        /// Performs an implicit conversion from T2 to <see cref="AnyOf{T1, T2, TRoot}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AnyOf<T1, T2, TRoot>(T2 value)
            => new(value);
    }

    /// <summary>
    /// Represents a value which can be one of multiple types all of which
    /// must be derived from the root type. This class supports implicit casting
    /// from the supported types.
    /// </summary>
    /// <typeparam name="T1">The first possible type.</typeparam>
    /// <typeparam name="T2">The second possible type.</typeparam>
    /// /// <typeparam name="T3">The third possible type.</typeparam>
    /// <typeparam name="TRoot">The type of the root.</typeparam>
    public class AnyOf<T1, T2, T3, TRoot>
        where T1 : TRoot
        where T2 : TRoot
        where T3 : TRoot
    {
        private AnyOf(TRoot value) => Value = value;

        /// <summary>
        /// Creates an instance using the root ancestor, used when interfaces
        /// are needed and implicit conversion cannot be used.
        /// </summary>
        /// <param name="value">The value to create.</param>
        /// <returns>New AnyOf instance.</returns>
        public static AnyOf<T1, T2, T3, TRoot> Of(TRoot value) => new(value);

        /// <summary>
        /// Gets the value for this instance.
        /// </summary>
        public TRoot Value { get; }

        /// <summary>
        /// Performs an implicit conversion from T1 to <see cref="AnyOf{T1, T2, T3, TRoot}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AnyOf<T1, T2, T3, TRoot>(T1 value)
            => new(value);

        /// <summary>
        /// Performs an implicit conversion from T2 to <see cref="AnyOf{T1, T2, T3, TRoot}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AnyOf<T1, T2, T3, TRoot>(T2 value)
            => new(value);

        /// <summary>
        /// Performs an implicit conversion from T2 to <see cref="AnyOf{T1, T2, T3, TRoot}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AnyOf<T1, T2, T3, TRoot>(T3 value)
            => new(value);
    }

}
