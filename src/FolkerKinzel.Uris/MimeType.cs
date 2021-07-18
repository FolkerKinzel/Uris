using System;
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
    public readonly struct MimeType : IEquatable<MimeType>
    {
        private readonly ReadOnlyMemory<char> _mediaType;
        private readonly ReadOnlyMemory<char> _subType;
        private readonly ReadOnlyMemory<char> _parameters;

        internal const int StringLength = 64;

        private MimeType(ReadOnlyMemory<char> mediaType, ReadOnlyMemory<char> subType, ReadOnlyMemory<char> parameters)
        {
            this._mediaType = mediaType.Trim();
            this._subType = subType.Trim();
            this._parameters = parameters;
        }

        #region Public Instance Members

        #region Properties

        /// <summary>
        /// Medientyp (Nie <c>null</c>.)
        /// </summary>
        public ReadOnlySpan<char> MediaType => _mediaType.Span;

        /// <summary>
        /// Subtyp (Nie <c>null</c>.)
        /// </summary>
        public ReadOnlySpan<char> SubType => _subType.Span;

        /// <summary>
        /// Parameter (Nie <c>null</c>.)
        /// </summary>
        public IEnumerable<MimeTypeParameter> Parameters => ParseParameters();


        public bool IsEmpty => _mediaType.IsEmpty;

        #endregion

        #region Methods

        public bool IsTextMediaType()
        {
            ReadOnlySpan<char> text = stackalloc char[] { 't', 'e', 'x', 't' };
            return text.Equals(MediaType, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsTextPlainType()
        {
            if (IsTextMediaType())
            {
                ReadOnlySpan<char> plain = stackalloc char[] { 'p', 'l', 'a', 'i', 'n' };
                return plain.Equals(SubType, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public string GetFileTypeExtension()
            => MimeCache.GetFileTypeExtension(ToString(false));

        
        /// <summary>
        /// Erstellt eine <see cref="string"/>-Repräsentation des <see cref="MimeType"/>-Objekts.
        /// </summary>
        /// <returns>Eine <see cref="string"/>-Repräsentation des <see cref="MimeType"/>-Objekts.</returns>
        public override string ToString() => ToString(true);

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public string ToString(bool includeParameters)
        {
            var sb = new StringBuilder(StringLength);
            _ = AppendTo(sb, includeParameters);
            return sb.ToString();
        }

        public StringBuilder AppendTo(StringBuilder sb, bool includeParameters = true)
        {
            if (sb is null)
            {
                throw new ArgumentNullException(nameof(sb));
            }

            if (IsEmpty)
            {
                return sb;
            }

            _ = sb.EnsureCapacity(sb.Length + StringLength);
            _ = sb.Append(MediaType).Append('/').Append(SubType).ToLowerInvariant();

            if (includeParameters)
            {
                foreach (MimeTypeParameter parameter in Parameters)
                {
                    parameter.AppendTo(sb);
                }
            }

            return sb;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public override int GetHashCode()
        {
            var hash = new HashCode();

            ReadOnlySpan<char> mediaTypeSpan = MediaType;
            for (int i = 0; i < mediaTypeSpan.Length; i++)
            {
                hash.Add(char.ToLowerInvariant(mediaTypeSpan[i]));
            }

            ReadOnlySpan<char> subTypeSpan = SubType;
            for (int j = 0; j < subTypeSpan.Length; j++)
            {
                hash.Add(char.ToLowerInvariant(subTypeSpan[j]));
            }

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            IOrderedEnumerable<MimeTypeParameter> thisParameters;

            if (MediaType.Equals("text".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                var asciiCharsetParameter = new MimeTypeParameter("charset".AsMemory(), "us-ascii".AsMemory());
                bool predicate(MimeTypeParameter x) => x.Equals(asciiCharsetParameter);

                thisParameters = Parameters.SkipWhile(predicate).OrderBy(KeySelector, comparer);
            }
            else
            {
                thisParameters = Parameters.OrderBy(KeySelector, comparer);
            }

            foreach (MimeTypeParameter parameter in thisParameters)
            {
                hash.Add(parameter);
            }

            return hash.ToHashCode();
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool Equals(MimeType other)
        {
            if (!MediaType.Equals(other.MediaType, StringComparison.OrdinalIgnoreCase) ||
               !SubType.Equals(other.SubType, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            IOrderedEnumerable<MimeTypeParameter> thisParameters;
            IOrderedEnumerable<MimeTypeParameter> otherParameters;

            if (MediaType.Equals("text".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                var asciiCharsetParameter = new MimeTypeParameter("charset".AsMemory(), "us-ascii".AsMemory());
                bool predicate(MimeTypeParameter x) => x.Equals(asciiCharsetParameter);

                thisParameters = Parameters.SkipWhile(predicate).OrderBy(KeySelector, comparer);
                otherParameters = other.Parameters.SkipWhile(predicate).OrderBy(KeySelector, comparer);
            }
            else
            {
                thisParameters = Parameters.OrderBy(KeySelector, comparer);
                otherParameters = other.Parameters.OrderBy(KeySelector, comparer);
            }

            return thisParameters.SequenceEqual(otherParameters);
        }


        public override bool Equals(object? obj) => obj is MimeType type && Equals(type);

        #endregion
        
        #endregion

        #region Static Members

        public static MimeType Empty => default;

        #region Static Methods

        public static void ClearCache() => MimeCache.Clear();

        public static MimeType Parse(string? value)
            => value is null
                ? throw new ArgumentNullException(value)
                : TryParse(value.AsMemory(), out MimeType mediaType)
                    ? mediaType
                    : throw new ArgumentException(string.Format(Res.InvalidMediaType, nameof(value)), nameof(value));


        public static bool TryParse(string value, out MimeType mimeType)
        {
            if(value is null)
            {
                mimeType = default;
                return false;
            }
            return TryParse(value.AsMemory(), out mimeType);
        }

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

        public static bool operator ==(MimeType mediaType1, MimeType mediaType2) => mediaType1.Equals(mediaType2);
        public static bool operator !=(MimeType mediaType1, MimeType mediaType2) => !(mediaType1 == mediaType2);

        #endregion

        #endregion

        #region private

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
