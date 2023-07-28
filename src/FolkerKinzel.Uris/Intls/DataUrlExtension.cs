namespace FolkerKinzel.Uris.Intls;

/// <summary>
/// Extension methods, which support the <see cref="DataUrlInfo"/> structure.
/// </summary>
internal static class DataUrlExtension
{
    internal static StringBuilder AppendMediaType(this StringBuilder builder, MimeType mimeType)
    {
        string mimeString = mimeType.ToString(MimeFormats.Url);

        if (mimeString.StartsWith(DataUrlBuilder.DEFAULT_MEDIA_TYPE, StringComparison.Ordinal))
        {
            if (MimeTypeInfo.Parse(DataUrlBuilder.DEFAULT_MEDIA_TYPE).Equals(MimeTypeInfo.Parse(mimeString)))
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

        builder.Append(mimeString);
        return builder;
    }

}
