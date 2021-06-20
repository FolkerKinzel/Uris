using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using FolkerKinzel.URIs.Properties;

namespace FolkerKinzel.URIs
{
    /// <summary>
    /// Kapselt Informationen über einen Medientyp. RFC 2045, RFC 7231 Section 3.1.1.1
    /// </summary>
    public sealed class InternetMediaType
    {
        private const string DEFAULT_MEDIA_TYPE = "text";
        private const string DEFAULT_SUB_TYPE = "plain";
        private const string CHARSET_PARAMETER_NAME = "charset";
        //private const string DEFAULT_CHARSET = "us-ascii";

        private static readonly ReadOnlyDictionary<string, string> _emptyParameters
            = new(new Dictionary<string, string>(0));

        //private const string REGEX_PATTERN = @"(?P<main>\w+|\*)/(?P<sub>\w+|\*)(\s*;\s*(?P<param>\w+)=\s*=\s*(?P<val>\S+))?";

        private const string MASK_CHARS = "[ ()<>@,;:\\\"/[>]?=]";

        private InternetMediaType(string mediaType, string subType, ReadOnlyDictionary<string, string> parameters)
        {
            MediaType = mediaType;
            SubType = subType;
            Parameters = parameters;
        }

        public static InternetMediaType Parse(string value)
        {
            return TryParse(value, out InternetMediaType? imt)
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
                var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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

        private static bool ParseParameters(string value, int parameterSeparatorIndex, Dictionary<string, string> dic)
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




        /// <summary>
        /// Medientyp (Nie <c>null</c>.)
        /// </summary>
        public string MediaType { get; } = DEFAULT_MEDIA_TYPE;

        /// <summary>
        /// Subtyp (Nie <c>null</c>.)
        /// </summary>
        public string SubType { get; } = DEFAULT_SUB_TYPE;

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
    }
}
