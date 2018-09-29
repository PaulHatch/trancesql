using System;
using System.Collections.Generic;
using System.Text;

namespace TranceSql.Language
{
    public class Any<T1, T2>
    {
        private Any(object value) => Value = value;

        public object Value { get; }

        public static implicit operator Any<T1, T2>(T1 value)
            => new Any<T1, T2>(value);

        public static implicit operator Any<T1, T2>(T2 value)
            => new Any<T1, T2>(value);
    }

    public class AnyOf<T1, T2, TRoot>
        where T1 : TRoot
        where T2 : TRoot
    {
        private AnyOf(TRoot value) => Value = value;

        public TRoot Value { get; }

        public static implicit operator AnyOf<T1, T2, TRoot>(T1 value)
            => new AnyOf<T1, T2, TRoot>(value);

        public static implicit operator AnyOf<T1, T2, TRoot>(T2 value)
            => new AnyOf<T1, T2, TRoot>(value);
    }

    public class Any<T1, T2, T3>
    {
        private Any(object value) => Value = value;

        public object Value { get; }

        public static implicit operator Any<T1, T2, T3>(T1 value)
            => new Any<T1, T2, T3>(value);

        public static implicit operator Any<T1, T2, T3>(T2 value)
            => new Any<T1, T2, T3>(value);

        public static implicit operator Any<T1, T2, T3>(T3 value)
            => new Any<T1, T2, T3>(value);
    }

    public class Any<T1, T2, T3, T4>
    {
        private Any(object value) => Value = value;

        public object Value { get; }

        public static implicit operator Any<T1, T2, T3, T4>(T1 value)
            => new Any<T1, T2, T3, T4>(value);

        public static implicit operator Any<T1, T2, T3, T4>(T2 value)
            => new Any<T1, T2, T3, T4>(value);

        public static implicit operator Any<T1, T2, T3, T4>(T3 value)
            => new Any<T1, T2, T3, T4>(value);

        public static implicit operator Any<T1, T2, T3, T4>(T4 value)
            => new Any<T1, T2, T3, T4>(value);

    }
}
