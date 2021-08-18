using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes;

#if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris.Intls
{
    /// <summary>
    /// Extension methods, which support the <see cref="DataUrlInfo"/> structure.
    /// </summary>
    internal static class DataUrlExtension
    {
        internal static StringBuilder AppendMediaType(this StringBuilder builder, in MimeType mimeType)
        {
            if (mimeType.IsEmpty)
            {
                return builder;
            }

            if (mimeType.IsTextPlain)
            {
                MimeType defaultMediaType = DataUrl.DefaultMediaType();

                if (mimeType.Equals(in defaultMediaType))
                {
                    return builder;
                }

                foreach (MimeTypeParameter parameter in mimeType.Parameters)
                {
                    _ = builder.Append(';');
                    _ = parameter.AppendTo(builder, urlEncodedValue: true);
                }

                return builder;
            }

            return mimeType.AppendTo(builder, MimeTypeFormattingOptions.AlwaysUrlEncoded | MimeTypeFormattingOptions.IncludeParameters);
        }

    }
}
