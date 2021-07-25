﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FolkerKinzel.Uris.Intls;
using FolkerKinzel.Uris.Properties;

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    /// <summary>
    /// Represents a MIME type ("Internet Media Type") according to RFC 2045 and RFC 2046.
    /// </summary>
    public readonly struct MimeType : IEquatable<MimeType>
    {
        private readonly ReadOnlyMemory<char> _mediaType;
        private readonly ReadOnlyMemory<char> _subType;
        private readonly ReadOnlyMemory<char> _parameters;

        internal const int StringLength = 80;

        private MimeType(ReadOnlyMemory<char> mediaType, ReadOnlyMemory<char> subType, ReadOnlyMemory<char> parameters)
        {
            this._mediaType = mediaType.Trim();
            this._subType = subType.Trim();
            this._parameters = parameters;
        }

        #region Public Instance Members

        #region Properties

        /// <summary>
        /// Top-Level Media Type. (The left part of a MIME-Type. Never <c>null</c>.)
        /// </summary>
        public ReadOnlySpan<char> TopLevelMediaType => _mediaType.Span;

        /// <summary>
        /// Sub Type (The right part of a MIME-Type. Never <c>null</c>.)
        /// </summary>
        public ReadOnlySpan<char> SubType => _subType.Span;

        /// <summary>
        /// Parameters (Never <c>null</c>.)
        /// </summary>
        public IEnumerable<MimeTypeParameter> Parameters => ParseParameters();

        /// <summary>
        /// <c>true</c> if the instance contains no data.
        /// </summary>
        public bool IsEmpty => _mediaType.IsEmpty;

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the <see cref="TopLevelMediaType"/> of this instance equals "text".
        /// The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="TopLevelMediaType"/> of this instance equals "text".</returns>
        public bool IsTextMediaType()
            => TopLevelMediaType.Equals(stackalloc char[] { 't', 'e', 'x', 't' }, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Determines whether this instance is equal to the MIME type "text/plain". The parameters are not taken into account.
        /// The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if this instance is equal to "text/plain".</returns>
        public bool IsTextPlain()
            => IsTextMediaType() && SubType.Equals(stackalloc char[] { 'p', 'l', 'a', 'i', 'n' }, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Finds an appropriate file type extension for the <see cref="MimeType"/> instance.
        /// </summary>
        /// <returns>An appropriate file type extension for the <see cref="MimeType"/> instance.</returns>
        public string GetFileTypeExtension()
            => MimeCache.GetFileTypeExtension(ToString(false));


        /// <summary>
        /// Creates a complete <see cref="string"/> representation of the instance that includes the <see cref="Parameters"/>.
        /// </summary>
        /// <returns>A complete <see cref="string"/> representation of the instance that includes the <see cref="Parameters"/>.</returns>
        public override string ToString() => ToString(true);

        /// <summary>
        /// Creates a <see cref="string"/> representation of the instance.
        /// </summary>
        /// <param name="includeParameters">Pass <c>true</c> to include the <see cref="Parameters"/>; <c>false</c>, otherwise.</param>
        /// <returns>A <see cref="string"/> representation of the instance.</returns>
        public string ToString(bool includeParameters)
        {
            var sb = new StringBuilder(StringLength);
            _ = AppendTo(sb, includeParameters);
            return sb.ToString();
        }

        /// <summary>
        /// Appends a <see cref="string"/> representation of this instance to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/>.</param>
        /// <param name="includeParameters">Pass <c>true</c> to include the <see cref="Parameters"/>; <c>false</c>, otherwise.</param>
        /// <returns>A reference to <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        public StringBuilder AppendTo(StringBuilder builder, bool includeParameters = true)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (IsEmpty)
            {
                return builder;
            }

            _ = builder.EnsureCapacity(builder.Length + StringLength);
            _ = builder.Append(TopLevelMediaType).Append('/').Append(SubType).ToLowerInvariant();

            if (includeParameters)
            {
                foreach (MimeTypeParameter parameter in Parameters)
                {
                    parameter.AppendTo(builder);
                }
            }

            return builder;
        }

        /// <summary>
        /// Creates a hash code for this instance, which takes <see cref="Parameters"/> into account.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => GetHashCode(false);

        /// <summary>
        /// Creates a hash code for this instance and allows to specify whether or not
        /// the <see cref="Parameters"/> are taken into account.
        /// </summary>
        /// <param name="ignoreParameters">Pass <c>false</c> to take the <see cref="Parameters"/> into account; <c>true</c>, otherwise.</param>
        /// <returns>The hash code.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public int GetHashCode(bool ignoreParameters)
        {
            var hash = new HashCode();

            ReadOnlySpan<char> mediaTypeSpan = TopLevelMediaType;
            for (int i = 0; i < mediaTypeSpan.Length; i++)
            {
                hash.Add(char.ToLowerInvariant(mediaTypeSpan[i]));
            }

            ReadOnlySpan<char> subTypeSpan = SubType;
            for (int j = 0; j < subTypeSpan.Length; j++)
            {
                hash.Add(char.ToLowerInvariant(subTypeSpan[j]));
            }

            if (ignoreParameters)
            {
                return hash.ToHashCode();
            }

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            IOrderedEnumerable<MimeTypeParameter> thisParameters =
                IsTextMediaType()
                ? Parameters.SkipWhile(UsAsciiPredicate).OrderBy(KeySelector, comparer)
                : Parameters.OrderBy(KeySelector, comparer);

            foreach (MimeTypeParameter parameter in thisParameters)
            {
                hash.Add(parameter);
            }

            return hash.ToHashCode();
        }

        /// <summary>
        /// Determines whether this instance is equal to <paramref name="other"/>. The <see cref="Parameters"/>
        /// are taken into account.
        /// </summary>
        /// <param name="other">The <see cref="MimeType"/> instance to compare with.</param>
        /// <returns><c>true</c> if this  instance is equal to <paramref name="other"/>; <c>false</c>, otherwise.</returns>
        public bool Equals(MimeType other) => Equals(other, false);

        /// <summary>
        /// Determines whether this instance is equal to <paramref name="other"/> and allows to specify
        /// whether or not the <see cref="Parameters"/> are taken into account.
        /// </summary>
        /// <param name="other">The <see cref="MimeType"/> instance to compare with.</param>
        /// <param name="ignoreParameters">Pass <c>false</c> to take the <see cref="Parameters"/> into account;
        /// <c>true</c>, otherwise.</param>
        /// <returns><c>true</c> if this  instance is equal to <paramref name="other"/>; false, otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool Equals(MimeType other, bool ignoreParameters)
        {
            if (!TopLevelMediaType.Equals(other.TopLevelMediaType, StringComparison.OrdinalIgnoreCase) ||
               !SubType.Equals(other.SubType, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (ignoreParameters)
            {
                return true;
            }

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            IOrderedEnumerable<MimeTypeParameter> thisParameters;
            IOrderedEnumerable<MimeTypeParameter> otherParameters;

            if (IsTextMediaType())
            {
                thisParameters = Parameters.SkipWhile(UsAsciiPredicate).OrderBy(KeySelector, comparer);
                otherParameters = other.Parameters.SkipWhile(UsAsciiPredicate).OrderBy(KeySelector, comparer);
            }
            else
            {
                thisParameters = Parameters.OrderBy(KeySelector, comparer);
                otherParameters = other.Parameters.OrderBy(KeySelector, comparer);
            }

            return thisParameters.SequenceEqual(otherParameters);
        }

        /// <summary>
        /// Determines whether <paramref name="obj"/> is a <see cref="MimeType"/> struct whose
        /// content is equal to that of this instance. The <see cref="Parameters"/>
        /// are taken into account.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="MimeType"/> struct whose
        /// content is equal to that of this instance; <c>false</c>, otherwise.</returns>
        public override bool Equals(object? obj) => obj is MimeType type && Equals(type);

        #endregion

        #endregion

        #region Static Members

        /// <summary>
        /// Returns a <see cref="MimeType"/> struct, which contains no data.
        /// </summary>
        public static MimeType Empty => default;

        #region Static Methods

        /// <summary>
        /// Clears the cache that's used to make subsequent searches for a
        /// file type extension faster.
        /// </summary>
        public static void ClearCache() => MimeCache.Clear();

        /// <summary>
        /// Parses a <see cref="string"/> as <see cref="MimeType"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <returns>The <see cref="MimeType"/> instance, which <paramref name="value"/> represents.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="MimeType"/>.</exception>
        public static MimeType Parse(string value)
            => value is null
                ? throw new ArgumentNullException(value)
                : TryParse(value.AsMemory(), out MimeType mediaType)
                    ? mediaType
                    : throw new ArgumentException(string.Format(Res.InvalidMimeType, nameof(value)), nameof(value));


        /// <summary>
        /// Tries to parse a <see cref="string"/> as <see cref="MimeType"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <param name="mimeType">When the method successfully returns, the parameter contains the
        /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string? value, out MimeType mimeType)
        {
            if (value is null)
            {
                mimeType = default;
                return false;
            }
            return TryParse(value.AsMemory(), out mimeType);
        }

        /// <summary>
        /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="MimeType"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
        /// <param name="mimeType">When the method successfully returns, the parameter contains the
        /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
        public static bool TryParse(ReadOnlyMemory<char> value, out MimeType mimeType)
        {
            int parameterStartIndex = value.Span.IndexOf(';');

            ReadOnlyMemory<char> mediaPart = parameterStartIndex < 0 ? value : value.Slice(0, parameterStartIndex);

            const char mediaTypeSeparator = '/';

            int mediaTypeSeparatorIndex = value.Span.IndexOf(mediaTypeSeparator);

            if (mediaTypeSeparatorIndex == -1 || mediaTypeSeparatorIndex == value.Length)
            {
                mimeType = default;
                return false;
            }

            mimeType = new MimeType(
                mediaPart.Slice(0, mediaTypeSeparatorIndex),
                mediaPart.Slice(mediaTypeSeparatorIndex + 1),
                parameterStartIndex < 0 ? ReadOnlyMemory<char>.Empty : value.Slice(parameterStartIndex + 1));

            return true;
        }

        /// <summary>
        /// Creates an appropriate <see cref="MimeType"/> instance for a given
        /// file type extension.
        /// </summary>
        /// <param name="fileTypeExtension">The file type extension to search for.</param>
        /// <returns>An appropriate <see cref="MimeType"/> instance for <paramref name="fileTypeExtension"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileTypeExtension"/> is <c>null</c>.</exception>
        public static MimeType FromFileTypeExtension(string fileTypeExtension)
        {
            if (fileTypeExtension is null)
            {
                throw new ArgumentNullException(nameof(fileTypeExtension));
            }
            else
            {
                _ = TryParse(MimeCache.GetMimeType(fileTypeExtension).AsMemory(), out MimeType inetMediaType);
                return inetMediaType;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="MimeType"/> instances are equal.
        /// The <see cref="Parameters"/> are taken into account.
        /// </summary>
        /// <param name="mimeType1">The first <see cref="MimeType"/> to compare.</param>
        /// <param name="mimeType2">The second <see cref="MimeType"/> to compare.</param>
        /// <returns><c>true</c> if <paramref name="mimeType1"/> and <paramref name="mimeType2"/> are equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(MimeType mimeType1, MimeType mimeType2) => mimeType1.Equals(mimeType2);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="MimeType"/> instances are not equal.
        /// The <see cref="Parameters"/> are taken into account.
        /// </summary>
        /// <param name="mimeType1">The first <see cref="MimeType"/> to compare.</param>
        /// <param name="mimeType2">The second <see cref="MimeType"/> to compare.</param>
        /// <returns><c>true</c> if <paramref name="mimeType1"/> and <paramref name="mimeType2"/> are not equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(MimeType mimeType1, MimeType mimeType2) => !(mimeType1 == mimeType2);

        #endregion

        #endregion

        #region private

        private static bool UsAsciiPredicate(MimeTypeParameter x) => x.IsAsciiCharsetParameter();

        private static string KeySelector(MimeTypeParameter parameter) => parameter.Key.ToString();


        private IEnumerable<MimeTypeParameter> ParseParameters()
        {
            int parameterStartIndex = 0;
            do
            {
                if (TryParseParameter(ref parameterStartIndex, out MimeTypeParameter parameter))
                {
                    yield return parameter;
                }
            }
            while (parameterStartIndex != -1);
        }


        private bool TryParseParameter(ref int parameterStartIndex, out MimeTypeParameter parameter)
        {
            int nextParameterSeparatorIndex = GetNextParameterSeparatorIndex(_parameters.Span.Slice(parameterStartIndex));
            ReadOnlyMemory<char> currentParameterString;

            if (nextParameterSeparatorIndex < 0) // last parameter
            {
                currentParameterString = _parameters.Slice(parameterStartIndex);
                parameterStartIndex = -1;
            }
            else
            {
                currentParameterString = _parameters.Slice(parameterStartIndex, nextParameterSeparatorIndex);
                parameterStartIndex += nextParameterSeparatorIndex + 1;
            }

            return MimeTypeParameter.TryParse(currentParameterString, out parameter);
        }


        private static int GetNextParameterSeparatorIndex(ReadOnlySpan<char> value)
        {
            bool isInQuotes = false;

            for (int i = 0; i < value.Length; i++)
            {
                char current = value[i];

                if (current == '"')
                {
                    isInQuotes = !isInQuotes;
                }

                if (isInQuotes)
                {
                    continue;
                }

                if (current == ';')
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

    }
}
