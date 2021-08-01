using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// <remarks>
    /// <note type="tip">
    /// <para>
    /// <see cref="MimeType"/> is a quite large structure. Pass it to other methods by reference (in, ref or out parameters in C#)!
    /// </para>
    /// <para>
    /// If you intend to hold a <see cref="MimeType"/> for a long time in memory and if this <see cref="MimeType"/> is parsed
    /// from a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from a very long <see cref="string"/>, 
    /// keep in mind, that the <see cref="MimeType"/> holds a reference to that <see cref="string"/>. Consider in this case to make
    /// a copy of the <see cref="MimeType"/> structure with <see cref="MimeType.Clone"/>: The copy is built on a separate <see cref="string"/>,
    /// which is only as long as needed.
    /// </para>
    /// </note>
    /// </remarks>
    public readonly struct MimeType : IEquatable<MimeType>, ICloneable
    {
        private readonly ReadOnlyMemory<char> _mimeTypeString;

        // Stores all indexes in one int.
        // | TopLevlMediaTp Length |  SubType Start  |  SubType Length  |  Parameters Start  |
        // |       Byte 4          |     Byte 3      |       Byte 2     |     Byte 1         |
        private readonly int _idx;

        internal const int StringLength = 80;

        private const int SUB_TYPE_LENGTH_SHIFT = 8;
        private const int SUB_TYPE_START_SHIFT = 16;
        private const int TOP_LEVEL_MEDIA_TYPE_LENGTH_SHIFT = 24;

        private MimeType(in ReadOnlyMemory<char> mimeTypeString, int idx)
        {
            this._mimeTypeString = mimeTypeString;
            this._idx = idx;
        }

        #region Public Instance Members

        #region Properties

        /// <summary>
        /// Top-Level Media Type. (The left part of a MIME-Type.)
        /// </summary>
        public ReadOnlySpan<char> TopLevelMediaType => _mimeTypeString.Span.Slice(0, (_idx >> TOP_LEVEL_MEDIA_TYPE_LENGTH_SHIFT) & 0xFF);

        /// <summary>
        /// Sub Type (The right part of a MIME-Type.)
        /// </summary>
        public ReadOnlySpan<char> SubType => _mimeTypeString.Span.Slice((_idx >> SUB_TYPE_START_SHIFT) & 0xFF, (_idx >> SUB_TYPE_LENGTH_SHIFT) & 0xFF);

        /// <summary>
        /// Parameters (Never <c>null</c>.)
        /// </summary>
        public IEnumerable<MimeTypeParameter> Parameters => ParseParameters();

        /// <summary>
        /// <c>true</c> if the instance contains no data.
        /// </summary>
        public bool IsEmpty => TopLevelMediaType.IsEmpty;

        #endregion

        #region Methods

        #region ICloneable

        object ICloneable.Clone() => throw new NotImplementedException();

        public MimeType Clone()
        {
            if(this.IsEmpty)
            {
                return default;
            }

            ReadOnlyMemory<char> memory = ToString().AsMemory();
            _ = TryParse(ref memory, out MimeType mimeType);

            return mimeType;
        }
        #endregion

        /// <summary>
        /// Determines whether the <see cref="TopLevelMediaType"/> of this instance equals "text".
        /// The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="TopLevelMediaType"/> of this instance equals "text".</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool IsText()
            => TopLevelMediaType.Equals("text".AsSpan(), StringComparison.OrdinalIgnoreCase);


        /// <summary>
        /// Determines whether this instance is equal to the MIME type "text/plain". The parameters are not taken into account.
        /// The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if this instance is equal to "text/plain".</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool IsTextPlain()
            => IsText() && SubType.Equals("plain".AsSpan(), StringComparison.OrdinalIgnoreCase);

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
            int insertStartIndex = builder.Length;
            _ = builder.Append(TopLevelMediaType).Append('/').Append(SubType).ToLowerInvariant(insertStartIndex);

            if (includeParameters)
            {
                foreach (MimeTypeParameter parameter in Parameters)
                {
                    _ = builder.Append(';');
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
                IsText()
                ? Parameters.SkipWhile(UsAsciiPredicate).OrderBy(KeySelector, comparer)
                : Parameters.OrderBy(KeySelector, comparer);

            foreach (MimeTypeParameter parameter in thisParameters)
            {
                hash.Add(parameter);
            }

            return hash.ToHashCode();
        }

        /// <summary>
        /// Determines whether the value of this instance is equal to the value of <paramref name="other"/>. The <see cref="Parameters"/>
        /// are taken into account.
        /// </summary>
        /// <param name="other">The <see cref="MimeType"/> instance to compare with.</param>
        /// <returns><c>true</c> if this the value of this instance is equal to that of <paramref name="other"/>; <c>false</c>, otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(MimeType other) => Equals(in other, false);

        /// <summary>
        /// Determines whether the value of this instance is equal to the value of <paramref name="other"/>. The <see cref="Parameters"/>
        /// are taken into account.
        /// </summary>
        /// <param name="other">The <see cref="MimeType"/> instance to compare with.</param>
        /// <returns><c>true</c> if this the value of this instance is equal to that of <paramref name="other"/>; <c>false</c>, otherwise.</returns>
        [CLSCompliant(false)]
        public bool Equals(in MimeType other) => Equals(in other, false);

        ///// <summary>
        ///// Determines whether this instance is equal to <paramref name="other"/> and allows to specify
        ///// whether or not the <see cref="Parameters"/> are taken into account.
        ///// </summary>
        ///// <param name="other">The <see cref="MimeType"/> instance to compare with.</param>
        ///// <param name="ignoreParameters">Pass <c>false</c> to take the <see cref="Parameters"/> into account;
        ///// <c>true</c>, otherwise.</param>
        ///// <returns><c>true</c> if this  instance is equal to <paramref name="other"/>; false, otherwise.</returns>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public bool Equals(MimeType other, bool ignoreParameters) => Equals(in other, ignoreParameters);

        /// <summary>
        /// Determines whether this instance is equal to <paramref name="other"/> and allows to specify
        /// whether or not the <see cref="Parameters"/> are taken into account.
        /// </summary>
        /// <param name="other">The <see cref="MimeType"/> instance to compare with.</param>
        /// <param name="ignoreParameters">Pass <c>false</c> to take the <see cref="Parameters"/> into account;
        /// <c>true</c>, otherwise.</param>
        /// <returns><c>true</c> if this  instance is equal to <paramref name="other"/>; false, otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        //[CLSCompliant(false)]
        public bool Equals(in MimeType other, bool ignoreParameters)
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

            if (IsText())
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
        /// Determines whether <paramref name="obj"/> is a <see cref="MimeType"/> structure whose
        /// value is equal to that of this instance. The <see cref="Parameters"/>
        /// are taken into account.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="MimeType"/> structure whose
        /// value is equal to that of this instance; <c>false</c>, otherwise.</returns>
        public override bool Equals(object? obj) => obj is MimeType type && Equals(in type, false);

        #endregion

        #endregion

        #region Static Members

        /// <summary>
        /// Returns a <see cref="MimeType"/> structure, which contains no data.
        /// </summary>
        public static MimeType Empty => default;

        #region Static Methods

        ///// <summary>
        ///// Clears the cache that's used to make subsequent searches for a
        ///// file type extension faster.
        ///// </summary>
        //public static void ClearCache() => MimeCache.Clear();

        /// <summary>
        /// Parses a <see cref="string"/> as <see cref="MimeType"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <returns>The <see cref="MimeType"/> instance, which <paramref name="value"/> represents.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="MimeType"/>.</exception>
        public static MimeType Parse(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(value);
            }

            ReadOnlyMemory<char> memory = value.AsMemory();

            return TryParse(ref memory, out MimeType mediaType)
                    ? mediaType
                    : throw new ArgumentException(string.Format(Res.InvalidMimeType, nameof(value)), nameof(value));
        }


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

            ReadOnlyMemory<char> memory = value.AsMemory();

            return TryParse(ref memory, out mimeType);
        }

        /// <summary>
        /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="MimeType"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse. The method might replace the 
        /// passed instance with a smaller one. Make a copy of the argument in the calling method if this is 
        /// not desirable.</param>
        /// <param name="mimeType">When the method successfully returns, the parameter contains the
        /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
        public static bool TryParse(ref ReadOnlyMemory<char> value, out MimeType mimeType)
        {
            value = value.TrimStart();
            ReadOnlySpan<char> span = value.Span;

            int parameterStartIndex = span.IndexOf(';');

            if(parameterStartIndex > byte.MaxValue) // string too long
            {
                goto Failed;
            }
            else if (parameterStartIndex < 0) // No parameters.
            {
                parameterStartIndex = 0;
            }


            ReadOnlySpan<char> mediaPartSpan = parameterStartIndex < 1 ? span : span.Slice(0, parameterStartIndex);
            int mediaTypeSeparatorIndex = mediaPartSpan.IndexOf('/');

            if (mediaTypeSeparatorIndex < 1)
            {
                goto Failed;
            }

            int topLevelMediaTypeLength = mediaPartSpan.Slice(0, mediaTypeSeparatorIndex).GetTrimmedLength();

            if (topLevelMediaTypeLength is 0 or > sbyte.MaxValue)
            {
                goto Failed;
            }

            int subTypeStart = mediaTypeSeparatorIndex + 1;
            subTypeStart += mediaPartSpan.Slice(subTypeStart).GetTrimmedStart();

            if (subTypeStart == mediaPartSpan.Length || subTypeStart > byte.MaxValue)
            {
                goto Failed;
            }

            int subTypeLength = mediaPartSpan.Slice(subTypeStart).GetTrimmedLength();

            int idx = topLevelMediaTypeLength << TOP_LEVEL_MEDIA_TYPE_LENGTH_SHIFT;
            idx |= subTypeStart << SUB_TYPE_START_SHIFT;
            idx |= subTypeLength << SUB_TYPE_LENGTH_SHIFT;
            idx |= parameterStartIndex;

            mimeType = new MimeType(
                in value,
                idx);

            return true;

/////////////////////////////////////////////////////////////
Failed:
            mimeType = default;
            return false;
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
                ReadOnlyMemory<char> memory = MimeCache.GetMimeType(fileTypeExtension).AsMemory();
                _ = TryParse(ref memory, out MimeType inetMediaType);
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
        public static bool operator ==(MimeType mimeType1, MimeType mimeType2) => mimeType1.Equals(in mimeType2, false);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="MimeType"/> instances are not equal.
        /// The <see cref="Parameters"/> are taken into account.
        /// </summary>
        /// <param name="mimeType1">The first <see cref="MimeType"/> to compare.</param>
        /// <param name="mimeType2">The second <see cref="MimeType"/> to compare.</param>
        /// <returns><c>true</c> if <paramref name="mimeType1"/> and <paramref name="mimeType2"/> are not equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(MimeType mimeType1, MimeType mimeType2) => !mimeType1.Equals(in mimeType2, false);

        #endregion

        #endregion

        #region private

        private static bool UsAsciiPredicate(MimeTypeParameter x) => x.IsAsciiCharsetParameter();

        private static string KeySelector(MimeTypeParameter parameter) => parameter.Key.ToString();


        private IEnumerable<MimeTypeParameter> ParseParameters()
        {
            int parameterStartIndex = _idx & 0xFF;

            if(parameterStartIndex < 1)
            {
                yield break;
            }

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
            int nextParameterSeparatorIndex = GetNextParameterSeparatorIndex(_mimeTypeString.Span.Slice(parameterStartIndex));
            ReadOnlyMemory<char> currentParameterString;

            if (nextParameterSeparatorIndex < 0) // last parameter
            {
                currentParameterString = _mimeTypeString.Slice(parameterStartIndex);
                parameterStartIndex = -1;
            }
            else
            {
                currentParameterString = _mimeTypeString.Slice(parameterStartIndex, nextParameterSeparatorIndex);
                parameterStartIndex += nextParameterSeparatorIndex + 1;
            }

            return MimeTypeParameter.TryParse(ref currentParameterString, out parameter);
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
