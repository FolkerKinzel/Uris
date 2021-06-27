using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;
using FolkerKinzel.Uris.Properties;

#if NETSTANDARD2_0
using FolkerKinzel.Strings;
#endif

namespace FolkerKinzel.Uris
{
    /// <summary>
    /// Kapselt Informationen über einen Medientyp. (RFC 2045, 2046, RFC 7231 Section 3.1.1.1)
    /// </summary>
    public sealed class InternetMediaType : IEquatable<InternetMediaType?>
    {
        internal const string TEXT_MEDIA_TYPE = "text";
        internal const string PLAIN_SUB_TYPE = "plain";
        internal const string CHARSET_PARAMETER_NAME = "charset";
        private const string DEFAULT_CHARSET = "us-ascii";

        private static readonly ReadOnlyDictionary<string, string> _emptyParameters
            = new(new Dictionary<string, string>(0));

        private const string MASK_CHARS = "[ ()<>@,;:\\\"/[>]?=]";

        internal InternetMediaType() : this(TEXT_MEDIA_TYPE, PLAIN_SUB_TYPE, _emptyParameters) { }


        private InternetMediaType(string mediaType, string subType, ReadOnlyDictionary<string, string> parameters)
        {
            MediaType = mediaType;
            SubType = subType;
            Parameters = parameters;
        }


        public static InternetMediaType Parse(string value)
        {
            return value is null
                ? throw new ArgumentNullException(nameof(value))
                : TryParse(value, out InternetMediaType? imt)
                    ? imt
                    : throw new ArgumentException(string.Format(Res.InvalidMediaType, nameof(value)));
        }


        public static bool TryParse(string value, [NotNullWhen(true)] out InternetMediaType? internetMediaType)
        {
            internetMediaType = default;

            if (value is null)
            {
                return false;
            }


            value = Regex.Replace(value, @"\s+", string.Empty, RegexOptions.None);

            int parameterSeparatorIndex = value.IndexOf(';');
            string mediaPart = parameterSeparatorIndex != -1 ? value.Substring(0, parameterSeparatorIndex).ToLowerInvariant() : value.ToLowerInvariant();

            string[] mediaArr = mediaPart.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (mediaArr.Length != 2)
            {
                throw new ArgumentException(string.Format(Res.InvalidMediaType, nameof(value)));
            }

            string mediaType = mediaArr[0];
            string subType = mediaArr[1];
            ReadOnlyDictionary<string, string> parameters = _emptyParameters;

            if (parameterSeparatorIndex != -1)
            {
                var dic = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                if (ParseParameters(value, parameterSeparatorIndex, dic))
                {
                    parameters = new ReadOnlyDictionary<string, string>(dic);
                }
                else
                {
                    return false;
                }
            }

            internetMediaType = new InternetMediaType(mediaType, subType, parameters);
            return true;
        }

        /// <summary>
        /// Medientyp (Nie <c>null</c>.)
        /// </summary>
        public string MediaType { get; } = TEXT_MEDIA_TYPE;

        /// <summary>
        /// Subtyp (Nie <c>null</c>.)
        /// </summary>
        public string SubType { get; } = PLAIN_SUB_TYPE;

        /// <summary>
        /// Parameter (Nie <c>null</c>.)
        /// </summary>
        public ReadOnlyDictionary<string, string> Parameters { get; } = _emptyParameters;

        /// <summary>
        /// Erstellt eine <see cref="string"/>-Repräsentation des <see cref="InternetMediaType"/>-Objekts.
        /// </summary>
        /// <returns>Eine <see cref="string"/>-Repräsentation des <see cref="InternetMediaType"/>-Objekts.</returns>
        public override string ToString() => ToString(true);


        public string ToString(bool includeParameters)
        {
            var sb = new StringBuilder();

            _ = sb.Append(MediaType).Append('/').Append(SubType);

            if (includeParameters)
            {
                foreach (KeyValuePair<string, string> parameter in Parameters)
                {
                    _ = sb.Append(';').Append(parameter.Key).Append('=');

                    // RFC 2045 Section 5.1 "tspecials"
                    _ = Regex.IsMatch(parameter.Value, MASK_CHARS) ? sb.Append('"').Append(parameter.Value).Append('"') : sb.Append(parameter.Value);

                }
            }

            return sb.ToString();
        }

        public override bool Equals(object? obj) => obj is InternetMediaType other && Equals(other);


        public bool Equals(InternetMediaType? other)
        {
            if (other is null || !MediaType.Equals(other.MediaType, StringComparison.Ordinal) || !SubType.Equals(other.SubType, StringComparison.Ordinal))
            {
                return false;
            }

            var thisList = Parameters.ToList();
            var otherList = Parameters.ToList();


            if (MediaType == TEXT_MEDIA_TYPE)
            {
                RemoveCharsetUsAscii(thisList);
                RemoveCharsetUsAscii(otherList);
            }

            if (thisList.Count != otherList.Count)
            {
                return false;
            }


            for (int i = 0; i < thisList.Count; i++)
            {
                KeyValuePair<string, string> kvp1 = thisList[i];
                KeyValuePair<string, string> kvp2 = otherList[i];

                if(!kvp1.Key.Equals(kvp2.Key) || !kvp1.Value.Equals(kvp2.Value))
                {
                    return false;
                }
            }

            return true;

            ////////////////////////////////////////////////

            static void RemoveCharsetUsAscii(List<KeyValuePair<string, string>> list)
            {
                int index = list.FindIndex(kvp => kvp.Key.Equals(CHARSET_PARAMETER_NAME, StringComparison.Ordinal) && kvp.Value.Equals(DEFAULT_CHARSET, StringComparison.Ordinal));
                if (index != -1)
                {
                    list.RemoveAt(index);
                }
            }
        }

        public override int GetHashCode() => HashCode.Combine(MediaType.GetHashCode(), SubType.GetHashCode());


        public static bool operator ==(InternetMediaType? mediaType1, InternetMediaType? mediaType2) => mediaType1 is null ? mediaType2 is null : mediaType1.Equals(mediaType2);

        public static bool operator !=(InternetMediaType? mediaType1, InternetMediaType? mediaType2) => !(mediaType1 == mediaType2);



        private static bool ParseParameters(string value, int parameterSeparatorIndex, SortedDictionary<string, string> dic)
        {
            while (parameterSeparatorIndex != -1)
            {
                int parameterStart = parameterSeparatorIndex + 1;
                int keyValueSeparatorIndex = value.IndexOf('=', parameterStart);

                if (keyValueSeparatorIndex <= parameterStart)
                {
                    return false;
                }

                string key = value.Substring(parameterStart, keyValueSeparatorIndex).ToLowerInvariant();

                int valueStart = keyValueSeparatorIndex + 1;
                parameterSeparatorIndex = GetNextParameterSeparatorIndex(value, valueStart);

                string val = parameterSeparatorIndex == -1
                    ? value.Substring(valueStart).Trim('"')
                    : value.Substring(valueStart, parameterSeparatorIndex).Trim('"');

                // RFC 2046 Sect. 412: Charset-Parameter not case sensitive.
                if (key == CHARSET_PARAMETER_NAME)
                {
                    val = val.ToLowerInvariant();
                }

                dic[key] = val;
            }

            return true;

            /////////////////////////////////////////////////////////////////

            static int GetNextParameterSeparatorIndex(string value, int start)
            {
                bool isInQuotes = false;

                for (int j = start; j < value.Length; j++)
                {
                    char current = value[j];

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
                        return j;
                    }
                }

                return -1;
            }
        }


        public Task<string> GetFileTypeExtensionAsync(double cacheLifeTime = 5) 
            => Task.Run(() => MimeCache.GetFileTypeExtension(ToString(false), cacheLifeTime));


        public static async Task<InternetMediaType> GetFromFileTypeExtensionAsync(string fileTypeExtension, double cacheLifeTime = 5)
        {
            if (fileTypeExtension is null)
            {
                throw new ArgumentNullException(nameof(fileTypeExtension));
            }

            fileTypeExtension = fileTypeExtension.Trim();

            if(!fileTypeExtension.StartsWith('.'))
            {
                fileTypeExtension = $".{fileTypeExtension}";
            }

            string mime = await Task.Run(() => MimeCache.GetMimeType(fileTypeExtension, cacheLifeTime));

            return InternetMediaType.Parse(mime);
        }

    }
}
