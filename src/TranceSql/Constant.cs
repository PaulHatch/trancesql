using System;
using System.Globalization;
using System.Linq;

namespace TranceSql;

/// <summary>
/// Defines a constant. Consider using <see cref="Value"/> if applicable.
/// </summary>
public class Constant : ExpressionElement, ISqlElement
{
    private const string _stringWarning = "Warning, string constants are vulnerable to SQL injection attacks. Using a parameter is the preferred method of passing string values. The 'Value' class can be used to supply a string which will automatically be passed to the command as a parameter. To create string constant expression, call the 'Constant.Unsafe(string)' method.";
    private object? _value;
        
    /// <summary>
    /// Gets or sets the value of this constant. Setting this value directly
    /// to a <see cref="string"/> instance will result in a runtime error, you
    /// must use <see cref="Constant.Unsafe(string)"/> to define a string constant.
    /// </summary>
    public object? Value
    {
        get => _value;
        set
        {
            if (value is string)
            {
                throw new ArgumentException(_stringWarning);
            }

            _value = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Constant"/> class.
    /// </summary>
    public Constant() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Constant"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public Constant(object value) { Value = value; }

    /// <summary>
    /// This constructor is to prevent accidentally introducing a SQL injection
    /// vulnerability, requiring the use of <see cref="Constant.Unsafe(string)"/>
    /// to create string constants. Using this constructor will result in a compile
    /// time error.
    /// </summary>
    /// <param name="value">The value.</param>
    [Obsolete(_stringWarning, true)]
    public Constant(string value) {  }

    /// <summary>
    /// Creates a string constant. While this value will have quotes escaped, even
    /// escaped strings are vulnerable to SQL injection attacks and this method should 
    /// not be used with untrusted input.
    /// </summary>
    /// <param name="value">The string constant value.</param>
    /// <returns>A new string constant.</returns>
    public static Constant Unsafe(string value)
    {
        var result = new Constant
        {
            _value = value
        };
        return result;
    }


    // order from most to least common to minimize checks
    private static Type[] _numericTypes = new Type[]
    {
        typeof(int),
        typeof(long),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(uint),
        typeof(ulong)
    };

    void ISqlElement.Render(RenderContext context)
    {
        if (Value == null)
        {
            context.Write("NULL");
            return;
        }

        var type = Value.GetType();
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (type == typeof(bool))
        {
            context.Write(((bool)Value) ? "CAST(1 AS BIT)" : "CAST(0 AS BIT)");
        }
        else if (_numericTypes.Contains(type))
        {
            context.Write(string.Format(CultureInfo.InvariantCulture, "{0}", Value));
        }
        else if (type == typeof(DateTime))
        {
            context.Write(context.Dialect.FormatDate((DateTime)Value));
        }
        else if (type == typeof(DateTimeOffset))
        {
            context.Write(context.Dialect.FormatDate((DateTimeOffset)Value));
        }
        else if (type == typeof(string))
        {
            context.Write(context.Dialect.FormatString((string)Value));
        }
        else
        {
            throw new InvalidCommandException($"Unsupported constant type {type.Name}");
        }
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.RenderDebug();
}