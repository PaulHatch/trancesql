using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql
{
    /// <summary>
    /// Represents a value which can be one of multiple types. This
    /// class supports implicit casting from the supported types.
    /// </summary>
    /// <typeparam name="T1">The first possible type.</typeparam>
    /// <typeparam name="T2">The second possible type.</typeparam>
    public class Any<T1, T2>
    {
        private Any(object value) => Value = value;

        /// <summary>
        /// Gets the value for this instance.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Performs an implicit conversion from T1 to <see cref="Any{T1, T2}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Any<T1, T2>(T1 value)
            => new Any<T1, T2>(value);

        /// <summary>
        /// Performs an implicit conversion from T2 to <see cref="Any{T1, T2}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Any<T1, T2>(T2 value)
            => new Any<T1, T2>(value);
    }

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
            => new AnyOf<T1, T2, TRoot>(value);

        /// <summary>
        /// Performs an implicit conversion from T2 to <see cref="AnyOf{T1, T2, TRoot}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AnyOf<T1, T2, TRoot>(T2 value)
            => new AnyOf<T1, T2, TRoot>(value);
    }

    /// <summary>
    /// Represents a value which can be one of multiple types. This
    /// class supports implicit casting from the supported types.
    /// </summary>
    /// <typeparam name="T1">The first possible type.</typeparam>
    /// <typeparam name="T2">The second possible type.</typeparam>
    /// <typeparam name="T3">The third possible type.</typeparam>
    public class Any<T1, T2, T3>
    {
        private Any(object value) => Value = value;

        /// <summary>
        /// Gets the value for this instance.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Performs an implicit conversion from T1 to <see cref="Any{T1, T2, T3}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Any<T1, T2, T3>(T1 value)
            => new Any<T1, T2, T3>(value);

        /// <summary>
        /// Performs an implicit conversion from T2 to <see cref="Any{T1, T2, T3}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Any<T1, T2, T3>(T2 value)
            => new Any<T1, T2, T3>(value);

        /// <summary>
        /// Performs an implicit conversion from T3 to <see cref="Any{T1, T2, T3}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Any<T1, T2, T3>(T3 value)
            => new Any<T1, T2, T3>(value);
    }

    /// <summary>
    /// Represents a value which can be one of multiple types. This
    /// class supports implicit casting from the supported types.
    /// </summary>
    /// <typeparam name="T1">The first possible type.</typeparam>
    /// <typeparam name="T2">The second possible type.</typeparam>
    /// <typeparam name="T3">The third possible type.</typeparam>
    /// <typeparam name="T4">The fourth possible type.</typeparam>
    public class Any<T1, T2, T3, T4>
    {
        private Any(object value) => Value = value;

        /// <summary>
        /// Gets the value for this instance.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Performs an implicit conversion from T1 to <see cref="Any{T1, T2, T3, T4}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Any<T1, T2, T3, T4>(T1 value)
            => new Any<T1, T2, T3, T4>(value);

        /// <summary>
        /// Performs an implicit conversion from T2 to <see cref="Any{T1, T2, T3, T4}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Any<T1, T2, T3, T4>(T2 value)
            => new Any<T1, T2, T3, T4>(value);

        /// <summary>
        /// Performs an implicit conversion from T3 to <see cref="Any{T1, T2, T3, T4}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Any<T1, T2, T3, T4>(T3 value)
            => new Any<T1, T2, T3, T4>(value);

        /// <summary>
        /// Performs an implicit conversion from T4 to <see cref="Any{T1, T2, T3, T4}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Any<T1, T2, T3, T4>(T4 value)
            => new Any<T1, T2, T3, T4>(value);

    }
}
