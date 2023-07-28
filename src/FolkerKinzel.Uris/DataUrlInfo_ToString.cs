using FolkerKinzel.Uris.Intls;

namespace FolkerKinzel.Uris;

public readonly partial struct DataUrlInfo
{
    /// <summary>
    /// Creates a "data" URL (RFC 2397) representation of the instance.
    /// </summary>
    /// <returns>A "data" URL (RFC 2397) representation of the instance.</returns>
    public override string ToString()
    {
        var builder = new StringBuilder(ComputeCapacity());
        return AppendTo(builder).ToString();
    }


    /// <summary>
    /// Appends a "data" URL (RFC 2397) representation of the instance to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/>.</param>
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
    public StringBuilder AppendTo(StringBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (DataEncoding == DataEncoding.Base64 || IsEmpty)
        {
            _ = builder.EnsureCapacity(ComputeCapacity());
            _ = builder.Append(DataUrlBuilder.Protocol).AppendMediaType(MimeTypes.MimeType.Create(in _mimeType)).Append(DataUrlBuilder.Base64).Append(',').Append(Data);
        }
        else if (TryGetEmbeddedText(out string? text))
        {
            string urlString = DataUrlBuilder.FromText(text);
            _ = builder.Append(urlString);
        }
        else // URL encoded bytes
        {
            _ = TryGetEmbeddedBytes(out byte[]? bytes);
            string urlString = DataUrlBuilder.FromBytes(bytes, MimeTypes.MimeType.Create(in _mimeType));
            _ = builder.Append(urlString);
        }

        return builder;
    }

    private int ComputeCapacity()
    {
        int capacity = DataUrlBuilder.Protocol.Length + DataUrlBuilder.ESTIMATED_MIME_TYPE_LENGTH;

        if (DataEncoding == DataEncoding.Base64)
        {
            capacity += DataUrlBuilder.Base64.Length;
        }

        return capacity;
    }

}
