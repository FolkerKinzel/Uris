using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris.Intls
{
    /// <summary>
    /// Extension methods, which support the <see cref="DataUrl"/> structure.
    /// </summary>
    internal static class DataUrlExtension
    {
        internal static bool IsAscii(this string s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                char c = s[i];

                if (((int)c) > 127)
                {
                    return false;
                }
            }

            return true;
        }


        internal static StringBuilder AppendMediaType(this StringBuilder builder, in MimeType mediaType)
        {
            if (mediaType.IsEmpty)
            {
                return builder;
            }

            if (mediaType.IsTextPlain())
            {
                MimeType defaultMediaType = DataUrl.DefaultMediaType();

                if (mediaType.Equals(in defaultMediaType))
                {
                    return builder;
                }

                foreach (MimeTypeParameter parameter in mediaType.Parameters)
                {
                    parameter.AppendTo(builder);
                }

                return builder;
            }

            return mediaType.AppendTo(builder);
        }

    }
}
