﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;

namespace CliWrap.Builders;

/// <summary>
/// Builder that helps generate well-formed arguments string.
/// </summary>
public class ArgumentsBuilder
{
    private static readonly IFormatProvider DefaultFormatProvider = CultureInfo.InvariantCulture;

    private readonly SecureString _buffer = new();
    private readonly StringBuilder _mask = new();

    /// <summary>
    /// Adds the specified value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(string value, bool escape)
    {
        return AddInternal(value, escape);
    }
    
    /// <summary>
    /// Adds the specified value to the list of arguments.
    /// </summary>
    // TODO: (breaking change) remove in favor of optional parameter
    public ArgumentsBuilder Add(string value) => Add(value, true);

    /// <summary>
    /// Adds the specified values to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(IEnumerable<string> values, bool escape)
    {
        foreach (var value in values)
        {
            Add(value, escape);
        }
        return this;
    }

    /// <summary>
    /// Adds the specified values to the list of arguments.
    /// </summary>
    // TODO: (breaking change) remove in favor of optional parameter
    public ArgumentsBuilder Add(IEnumerable<string> values) =>
        Add(values, true);


    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(string value, bool escape, string mask)
    {
        return AddInternal(value, escape, mask);
    }

    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(string value, string mask) => Add(value, true, mask);
    
    /// <summary>
    /// Adds the specified value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(IFormattable value, IFormatProvider formatProvider, bool escape = true) =>
        Add(value.ToString(null, formatProvider), escape);

    /// <summary>
    /// Adds the specified value to the list of arguments.
    /// </summary>
    // TODO: (breaking change) remove in favor of other overloads
    public ArgumentsBuilder Add(IFormattable value, CultureInfo cultureInfo, bool escape) =>
        Add(value, (IFormatProvider) cultureInfo, escape);

    /// <summary>
    /// Adds the specified value to the list of arguments.
    /// </summary>
    // TODO: (breaking change) remove in favor of other overloads
    public ArgumentsBuilder Add(IFormattable value, CultureInfo cultureInfo) =>
        Add(value, cultureInfo, true);

    /// <summary>
    /// Adds the specified value to the list of arguments.
    /// The value is converted to string using invariant culture.
    /// </summary>
    public ArgumentsBuilder Add(IFormattable value, bool escape) =>
        Add(value, DefaultFormatProvider, escape);

    /// <summary>
    /// Adds the specified value to the list of arguments.
    /// The value is converted to string using invariant culture.
    /// </summary>
    // TODO: (breaking change) remove in favor of optional parameter
    public ArgumentsBuilder Add(IFormattable value) =>
        Add(value, true);

    /// <summary>
    /// Adds the specified values to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(IEnumerable<IFormattable> values, IFormatProvider formatProvider, bool escape = true)
    {
        foreach (var value in values)
            Add(value, formatProvider, escape);

        return this;
    }

    /// <summary>
    /// Adds the specified values to the list of arguments.
    /// </summary>
    // TODO: (breaking change) remove in favor of other overloads
    public ArgumentsBuilder Add(IEnumerable<IFormattable> values, CultureInfo cultureInfo, bool escape) =>
        Add(values, (IFormatProvider) cultureInfo, escape);

    /// <summary>
    /// Adds the specified values to the list of arguments.
    /// </summary>
    // TODO: (breaking change) remove in favor of other overloads
    public ArgumentsBuilder Add(IEnumerable<IFormattable> values, CultureInfo cultureInfo) =>
        Add(values, cultureInfo, true);

    /// <summary>
    /// Adds the specified values to the list of arguments.
    /// The values are converted to string using invariant culture.
    /// </summary>
    public ArgumentsBuilder Add(IEnumerable<IFormattable> values, bool escape) =>
        Add(values, DefaultFormatProvider, escape);

    /// <summary>
    /// Adds the specified values to the list of arguments.
    /// The values are converted to string using invariant culture.
    /// </summary>
    // TODO: (breaking change) remove in favor of optional parameter
    public ArgumentsBuilder Add(IEnumerable<IFormattable> values) =>
        Add(values, true);
    
    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add
        (IFormattable value,
        IFormatProvider formatProvider,
        bool escape,
        string mask) =>
        Add(value.ToString(null, formatProvider), escape, mask);

    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add
        (IFormattable value,
        CultureInfo cultureInfo,
        bool escape,
        string mask) =>
        Add(value, (IFormatProvider)cultureInfo, escape, mask);

    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(IFormattable value, CultureInfo cultureInfo, string mask) =>
        Add(value, cultureInfo, true, mask);
    
    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// The value is converted to string using invariant culture.
    /// </summary>
    public ArgumentsBuilder Add(IFormattable value, bool escape, string mask) =>
        Add(value, DefaultFormatProvider, escape, mask);

    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// The value is converted to string using invariant culture.
    /// </summary>
    public ArgumentsBuilder Add(IFormattable value, string mask) =>
        Add(value, true, mask);
    
    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(SensitiveString value) 
        => Add(value, true);
    
    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(SensitiveString value, bool escape)
    {
        var unsecureValue = value.UnsecureString ?? string.Empty;
        return AddInternal(unsecureValue, escape, value.ToString());
    }

    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(SecureString value)
        => Add(value, true);

    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(SecureString value, bool escape)
        => AddInternal(SecureStringHelper.MarshalToString(value) ?? string.Empty, escape, SensitiveString.DefaultMask);

    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(SecureString value, string mask)
        => Add(value, true, mask);

    /// <summary>
    /// Adds the specified sensitive value to the list of arguments.
    /// </summary>
    public ArgumentsBuilder Add(SecureString value, bool escape, string mask) 
        => AddInternal(SecureStringHelper.MarshalToString(value) ?? string.Empty, escape, mask);

    /// <summary>
    /// Builds the resulting arguments string.
    /// </summary>
    public SensitiveString Build() => new SensitiveString(_buffer, _mask.ToString());


    private ArgumentsBuilder AddInternal(string value, bool escape)
    {
        string escapedValue = escape ? Escape(value) : value;
        return AddInternal(escapedValue, escapedValue);
    }

    private ArgumentsBuilder AddInternal(string value, bool escape, string mask)
    {
        string escapedValue = escape ? Escape(value) : value;
        return AddInternal(escapedValue, mask);
    }

    private ArgumentsBuilder AddInternal(string value, string mask)
    {
        if (_buffer.Length > 0)
        {
            _buffer.AppendChar(' ');
            _mask.Append(' ');
        }
        foreach (char c in value)
        {
            _buffer.AppendChar(c);
        }
        _mask.Append(mask);

        return this;
    }

    private static string Escape(string argument)
    {
        // Implementation reference:
        // https://github.com/dotnet/runtime/blob/9a50493f9f1125fda5e2212b9d6718bc7cdbc5c0/src/libraries/System.Private.CoreLib/src/System/PasteArguments.cs#L10-L79

        // Short circuit if argument is clean and doesn't need escaping
        if (argument.Length != 0 && argument.All(c => !char.IsWhiteSpace(c) && c != '"'))
            return argument;

        var buffer = new StringBuilder();

        buffer.Append('"');

        for (var i = 0; i < argument.Length;)
        {
            var c = argument[i++];

            if (c == '\\')
            {
                var numBackSlash = 1;
                while (i < argument.Length && argument[i] == '\\')
                {
                    numBackSlash++;
                    i++;
                }

                if (i == argument.Length)
                {
                    buffer.Append('\\', numBackSlash * 2);
                }
                else if (argument[i] == '"')
                {
                    buffer.Append('\\', numBackSlash * 2 + 1);
                    buffer.Append('"');
                    i++;
                }
                else
                {
                    buffer.Append('\\', numBackSlash);
                }
            }
            else if (c == '"')
            {
                buffer.Append('\\');
                buffer.Append('"');
            }
            else
            {
                buffer.Append(c);
            }
        }

        buffer.Append('"');

        return buffer.ToString();
    }
}