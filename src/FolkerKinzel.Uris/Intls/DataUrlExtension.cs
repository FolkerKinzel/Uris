namespace FolkerKinzel.Uris.Intls;

/// <summary>
/// Extension methods, which support the <see cref="DataUrlInfo"/> structure.
/// </summary>
internal static class DataUrlExtension
{
    internal static StringBuilder AppendMediaType(this StringBuilder builder, MimeType mimeType)
    {
        if (mimeType.IsTextPlain)
        {
            if (MimeType.Create("text", "plain").Equals(mimeType))
            {
                return builder;
            }

            foreach (MimeTypeParameter parameter in mimeType.Parameters)
            {
                _ = builder.Append(';');
                parameter.AppendTo(builder, urlFormat: true);
            }

            return builder;
        }

        builder.Append(mimeType.ToString(MimeFormats.Url));
        return builder;
    }

}
