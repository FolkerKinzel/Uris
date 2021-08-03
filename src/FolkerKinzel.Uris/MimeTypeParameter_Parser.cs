using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeTypeParameter : IEquatable<MimeTypeParameter>, ICloneable
    {
        internal static bool TryParse(ref ReadOnlyMemory<char> value, out MimeTypeParameter parameter, out bool quoted)
        {
            quoted = false;

            value = value.Trim();

            if (value.Length == 0)
            {
                goto Failed;
            }

            ReadOnlySpan<char> span = value.Span;

            int keyValueSeparatorIndex = span.IndexOf('=');

            if (keyValueSeparatorIndex < 1)
            {
                goto Failed;
            }

            int valueStart = keyValueSeparatorIndex + 1;

            if (valueStart == span.Length)
            {
                goto Failed;
            }

            valueStart += span.Slice(valueStart).GetTrimmedStart();

            int keyLength = span.Slice(0, keyValueSeparatorIndex).GetTrimmedLength();

            if (keyLength is 0 or > KEY_LENGTH_MAX_VALUE)
            {
                goto Failed;
            }

            int languageStart = 0;
            int languageLength = 0;
            int charsetStart = 0;
            int charsetLength = 0;

            if (span[keyLength - 1] == '*')
            {
                --keyLength;

                if (keyLength is 0)
                {
                    goto Failed;
                }

                charsetStart = valueStart;

                bool startInitialized = false;
                for (int i = valueStart; i < span.Length; i++)
                {
                    char c = span[i];

                    if (c == '\'')
                    {
                        if (startInitialized)
                        {
                            languageLength = i - languageStart;
                            valueStart = i + 1;
                            break;
                        }
                        else
                        {
                            startInitialized = true;
                            charsetLength = i - valueStart;
                            languageStart = i + 1;
                        }
                    }
                }

                if (languageStart > LANGUAGE_START_MAX_VALUE ||
                    languageLength > LANGUAGE_LENGTH_MAX_VALUE ||
                    charsetStart > CHARSET_START_MAX_VALUE ||
                    charsetLength > CHARSET_LENGTH_MAX_VALUE)
                {
                    goto Failed;
                }
            }

            if (valueStart > VALUE_START_MAX_VALUE)
            {
                goto Failed;
            }

            // Masked Value:
            if (span[span.Length - 1] == '"')
            {
                quoted = true;
                if (span.Slice(valueStart).Contains('\\')) // Masked chars
                {
                    var builder = new StringBuilder(value.Length);
                    _ = builder.Append(value).Remove(builder.Length - 1, 1);

                    UnMask(builder, valueStart);

                    ReadOnlyMemory<char> mem = builder.ToString().AsMemory();
                    return TryParse(ref mem, out parameter, out _);
                }
                else // No masked chars - tspecials only
                {
                    value = value.Slice(0, value.Length - 1);
                    span = value.Span;
                    valueStart++;
                }
            }


            //ReadOnlySpan<char> valueSpan = span.Slice(valueStart);
            //if(!quoted && valueSpan.Contains('%'))
            //{
            //    Encoding encoding = TextEncodingConverter.GetEncoding(span.Slice(charsetStart, charsetLength).ToString());
            //    byte[] bytes = Encoding.ASCII.GetBytes(valueSpan.ToString());
            //    string result = encoding.GetString(WebUtility.UrlDecodeToBytes(bytes,0, bytes.Length));

            //    var sb = new StringBuilder(valueStart + result.Length);

            //    ReadOnlyMemory<char> memory = sb.Append(value.Slice(0, valueStart)).Append(result).ToString().AsMemory();
            //    return TryParse(ref memory, out parameter);
            //}


            int idx1 = (keyLength << KEY_LENGTH_SHIFT) |
            (charsetStart << CHARSET_START_SHIFT) |
            valueStart;

            int idx2 = (languageLength << LANGUAGE_LENGTH_SHIFT) |
            (languageStart << LANGUAGE_START_SHIFT) |
            charsetLength;

            parameter = new MimeTypeParameter(in value, idx1, idx2);

            return true;
///////////////////////////////
Failed:
            parameter = default;
            return false;
        }

        private static void UnMask(StringBuilder builder, int startOfValue)
        {
            bool quotesRemoved = false;
            for (int i = startOfValue; i < builder.Length; i++)
            {
                char c = builder[i];

                if (!quotesRemoved)
                {
                    //if (char.IsWhiteSpace(c))
                    //{
                    //    _ = builder.Remove(i--, 1);
                    //    continue;
                    //}
                    if (c == '"')
                    {
                        _ = builder.Remove(i--, 1);
                    }

                    quotesRemoved = true;
                }

                if (c == '\\')
                {
                    // after the mask char one entry can be skipped:
                    _ = builder.Remove(i, 1);
                }
            }
        }
    }
}
