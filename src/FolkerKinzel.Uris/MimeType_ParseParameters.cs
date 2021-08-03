using System;
using System.Collections.Generic;
using System.Data.Common;
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
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        #region ParseParameters

        private IEnumerable<MimeTypeParameter> ParseParameters()
        {
            int parameterStartIndex = _idx & 0xFF;

            if (parameterStartIndex < 1)
            {
                yield break;
            }

            //string charset = "";
            Encoding encoding = Encoding.UTF8;
            string currentKey = "";

            StringBuilder? sb = null;
            MimeTypeParameter concatenated;

            do
            {
                if (TryParseParameter(ref parameterStartIndex, out MimeTypeParameter parameter, out bool quoted))
                {
                    ReadOnlySpan<char> keySpan = parameter.Key;
                    int starIndex = keySpan.IndexOf('*');

                    if (starIndex > 0)
                    {
                        sb ??= new StringBuilder(MimeTypeParameter.StringLength);

                        keySpan = keySpan.Slice(0, starIndex + 1);
                        if (!currentKey.AsSpan().Equals(keySpan, StringComparison.OrdinalIgnoreCase))
                        {
                            currentKey = keySpan.ToString();
                            encoding = GetEncoding(parameter.Charset);

                            if (BuildConcatenated(sb, out concatenated))
                            {
                                yield return concatenated;
                            }
                            _ = sb.Append(currentKey).Append('=').Append(parameter.Charset).Append('\'').Append(parameter.Language).Append('\'');
                        }

                        // mit vorigem zusammensetzen
                        ReadOnlySpan<char> valueSpan = parameter.Value;
                        if (!quoted && valueSpan.Contains('%'))
                        {
                            byte[] bytes = Encoding.ASCII.GetBytes(valueSpan.ToString());
                            _ = sb.Append(encoding.GetString(WebUtility.UrlDecodeToBytes(bytes, 0, bytes.Length)));
                        }
                        else
                        {
                            _ = sb.Append(valueSpan);
                        }
                        continue;
                    }
                    else
                    {
                        if (BuildConcatenated(sb, out concatenated))
                        {
                            yield return concatenated;
                        }

                        currentKey = "";
                        encoding = GetEncoding(parameter.Charset);

                        if (RemoveUrlEncoding(encoding, parameter.Value, quoted, ref sb, ref parameter))
                        {
                            yield return parameter;
                        }
                    }
                }



            }
            while (parameterStartIndex != -1);

            if (BuildConcatenated(sb, out concatenated))
            {
                yield return concatenated;
            }
        }

        private static Encoding GetEncoding(ReadOnlySpan<char> charsetSpan) => charsetSpan.IsEmpty ? Encoding.UTF8 : TextEncodingConverter.GetEncoding(charsetSpan.ToString());

        private static bool BuildConcatenated(StringBuilder? sb, out MimeTypeParameter concatenated)
        {
            if (sb is not null && sb.Length != 0)
            {
                ReadOnlyMemory<char> mem = sb.ToString().AsMemory();
                _ = sb.Clear();

                return MimeTypeParameter.TryParse(ref mem, out concatenated, out _);
            }

            concatenated = default;
            return false;
        }

        private static bool RemoveUrlEncoding(Encoding encoding, ReadOnlySpan<char> valueSpan, bool quoted, ref StringBuilder? sb, ref MimeTypeParameter parameter)
        {
            if (!quoted && valueSpan.Contains('%'))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(valueSpan.ToString());
                string result = encoding.GetString(WebUtility.UrlDecodeToBytes(bytes, 0, bytes.Length));

                ReadOnlySpan<char> keySpan = parameter.Key;
                sb ??= new StringBuilder(keySpan.Length + 1 + result.Length);

                ReadOnlyMemory<char> memory = sb.Append(keySpan).Append('=').Append(result).ToString().AsMemory();

                return MimeTypeParameter.TryParse(ref memory, out parameter, out bool _);
            }

            return true;
        }


        private bool TryParseParameter(ref int parameterStartIndex, out MimeTypeParameter parameter, out bool quoted)
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

            return MimeTypeParameter.TryParse(ref currentParameterString, out parameter, out quoted);
        }


        private static int GetNextParameterSeparatorIndex(ReadOnlySpan<char> value)
        {
            bool isInQuotes = false;

            for (int i = 0; i < value.Length; i++)
            {
                char current = value[i];

                if (current == '\\') // Mask char: Skip one!
                {
                    i++;
                    continue;
                }

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
