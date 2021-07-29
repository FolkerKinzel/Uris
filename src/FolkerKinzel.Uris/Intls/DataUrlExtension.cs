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
    /// Extension methods, which support the <see cref="DataUrl"/> struct.
    /// </summary>
    public static class DataUrlExtension
    {
        internal static StringBuilder AppendMediaType(this StringBuilder builder, MimeType mediaType)
        {
            if (mediaType.IsEmpty || mediaType.Equals(DataUrl.DefaultMediaType()))
            {
                return builder;
            }

            if (mediaType.IsTextPlain())
            {
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
