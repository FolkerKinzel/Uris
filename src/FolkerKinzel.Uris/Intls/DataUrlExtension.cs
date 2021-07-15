using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETSTANDARD2_0
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris.Intls
{
    internal static class DataUrlExtension
    {
        internal static StringBuilder AppendProtocol(this StringBuilder sb)
        {
            ReadOnlySpan<char> protocol = stackalloc char[] { 'd', 'a', 't', 'a', ':' };
            return sb.Append(protocol);
        }

        internal static StringBuilder AppendMediaType(this StringBuilder builder, InternetMediaType mediaType)
        {
            if (mediaType.Equals(DataUrl.DefaultMediaType()))
            {
                return builder;
            }

            if (mediaType.IsTextPlainType())
            {
                foreach (MediaTypeParameter parameter in mediaType.Parameters)
                {
                    parameter.AppendTo(builder);
                }

                return builder;
            }

            return mediaType.AppendTo(builder);
        }

        internal static StringBuilder AppendBase64(this StringBuilder builder)
        {
            ReadOnlySpan<char> base64 = stackalloc char[] { ';', 'b', 'a', 's', 'e', '6', '4', ',' };
            return builder.Append(base64);
        }
    }
}
