using System.Text;
using FolkerKinzel.MimeTypes;

namespace FolkerKinzel.Uris.Intls;

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
            MimeType defaultMediaType = DataUrlBuilder.DefaultMediaType();

            if (mimeType.Equals(in defaultMediaType))
            {
                return builder;
            }

            foreach (MimeTypeParameter parameter in mimeType.Parameters())
            {
                _ = builder.Append(';');
                parameter.AppendTo(builder, alwaysUrlEncoded: true);
            }

            return builder;
        }

        mimeType.AppendTo(builder, FormattingOptions.AlwaysUrlEncoded);
        return builder;
    }

}
