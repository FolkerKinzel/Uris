using System;
using System.Collections.Generic;
using System.Text;
using FolkerKinzel.Uris.Intls;

#if NETSTANDARD2_0 || NETSTANDARD2_1
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    public readonly struct Parameter
    {
        private readonly ReadOnlyMemory<char> _key;
        private readonly ReadOnlyMemory<char> _value;



        public Parameter(ReadOnlyMemory<char> key, ReadOnlyMemory<char> value)
        {
            this._key = key;
            this._value = value;
        }

        public ReadOnlySpan<char> Key => _key.Span;
        public ReadOnlySpan<char> Value => _value.Span;
    }

    public readonly struct InternetMediaType2
    {
        private readonly ReadOnlyMemory<char> _mediaType;
        private readonly ReadOnlyMemory<char> _subType;
        private readonly ReadOnlyMemory<char> _parameters;

        internal const string CHARSET_PARAMETER_NAME = "charset";


        private InternetMediaType2(ReadOnlyMemory<char> mediaType, ReadOnlyMemory<char> subType, ReadOnlyMemory<char> parameters)
        {
            this._mediaType = mediaType;
            this._subType = subType;
            this._parameters = parameters;
        }

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
        public IEnumerable<Parameter> Parameters => ParseParameters();


        public static bool TryParse(ReadOnlyMemory<char> mediaTypeString, out InternetMediaType2 mediaType)
        {
            int parameterStartIndex = mediaTypeString.Span.IndexOf(';');

            ReadOnlyMemory<char> mediaPart = parameterStartIndex < 0 ? mediaTypeString : mediaTypeString.Slice(0, parameterStartIndex);

            const char mediaTypeSeparator = '/';

            int mediaTypeSeparatorIndex = mediaTypeString.Span.IndexOf(mediaTypeSeparator);

            if (mediaTypeSeparatorIndex == -1 || mediaTypeSeparatorIndex == mediaTypeString.Length)
            {
                mediaType = default;
                return false;
            }


            mediaType = new InternetMediaType2(
                mediaPart.Slice(0, mediaTypeSeparatorIndex).Trim(),
                mediaPart.Slice(mediaTypeSeparatorIndex + 1).Trim(),
                parameterStartIndex < 0 ? ReadOnlyMemory<char>.Empty : mediaTypeString.Slice(parameterStartIndex + 1));

            return true;
        }


        private IEnumerable<Parameter> ParseParameters()
        {
            int parameterStartIndex = 0;
            do
            {
                if (TryBuildParameter(ref parameterStartIndex, out Parameter parameter))
                {
                    yield return parameter;
                }
            }
            while (parameterStartIndex != -1);
        }

        private bool TryBuildParameter(ref int parameterStartIndex, out Parameter parameter)
        {
            int parameterLength = GetParameterLength(_parameters.Span.Slice(parameterStartIndex));
            ReadOnlyMemory<char> currentParameterString;

            if (parameterLength < 0)
            {
                currentParameterString = _parameters.Slice(parameterStartIndex);
                parameterStartIndex = -1;
            }
            else
            {
                currentParameterString = _parameters.Slice(parameterStartIndex, parameterLength);
                parameterStartIndex = parameterLength + 1;
            }

            int keyValueSeparatorIndex = currentParameterString.Span.IndexOf('=');

            if (keyValueSeparatorIndex < 1)
            {
                parameter = default;
                return false;
            }

            ReadOnlyMemory<char> value = currentParameterString.Slice(keyValueSeparatorIndex).Trim();

            ReadOnlySpan<char> span = value.Span;
            if (span.Length > 0 && span[0] == '"')
            {
                value = span.Length > 1 && span[span.Length - 1] == '"'
                            ? value.Slice(1, value.Length - 2)
                            : value.Slice(1);
            }

            parameter = new Parameter(currentParameterString.Slice(0, keyValueSeparatorIndex).Trim(), value);
            return true;
        }

        private static int GetParameterLength(ReadOnlySpan<char> value)
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


        /// <summary>
        /// Erstellt eine <see cref="string"/>-Repräsentation des <see cref="InternetMediaType"/>-Objekts.
        /// </summary>
        /// <returns>Eine <see cref="string"/>-Repräsentation des <see cref="InternetMediaType"/>-Objekts.</returns>
        public override string ToString() => ToString(true);


        public string ToString(bool includeParameters)
        {
            var sb = new StringBuilder();

            _ = sb.Append(MediaType).Append('/').Append(SubType).ToLowerInvariant();

            if (includeParameters)
            {
                // RFC 2045 Section 5.1 "tspecials"
                ReadOnlySpan<char> maskChars = stackalloc char[] {' ', '(', ')', '<', '>', '@', ',', ';', ':', '\\', '\"', '/', '[', '>', ']', '?', '=' };

                foreach (Parameter parameter in Parameters)
                {
                    _ = sb.Append(';');

                    int keyStart = sb.Length;
                    _ = sb.Append(parameter.Key).ToLowerInvariant(keyStart).Append('=');

                    
                    bool mask = parameter.Value.ContainsAny(maskChars);
                    {
                        if (mask)
                        {
                            _ = sb.Append('"');
                        }

                        int valueStart = sb.Length;
                        _ = parameter.Key.CompareTo(CHARSET_PARAMETER_NAME, StringComparison.OrdinalIgnoreCase) == 0
                            ? sb.Append(parameter.Value).ToLowerInvariant()
                            : sb.Append(parameter.Value);

                        if (mask)
                        {
                            _ = sb.Append('"');
                        }
                    }
                }
            }

            return sb.ToString();
        }


        public string GetFileTypeExtension()
            => MimeCache.GetFileTypeExtension(ToString(false));


        public static InternetMediaType FromFileTypeExtension(string fileTypeExtension)
        {
            return fileTypeExtension is null
                ? throw new ArgumentNullException(nameof(fileTypeExtension))
                : InternetMediaType.Parse(MimeCache.GetMimeType(fileTypeExtension));
        }

    }
}
