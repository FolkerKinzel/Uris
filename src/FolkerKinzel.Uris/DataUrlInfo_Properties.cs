namespace FolkerKinzel.Uris;

public readonly partial struct DataUrlInfo
{
    private readonly ReadOnlyMemory<char> _embeddedData;
    private readonly MimeTypeInfo _mimeType;

    /// <summary>
    /// Information about the internet media type of the embedded data.
    /// </summary>
    public MimeTypeInfo MimeTypeInfo => _mimeType;

    /// <summary>
    /// The encoding of the data in <see cref="Data"/>.
    /// </summary>
    public DataEncoding DataEncoding { get; }

    /// <summary>
    /// The part of the "data" URL, which contains the embedded data.
    /// </summary>
    public ReadOnlySpan<char> Data => _embeddedData.Span;

    /// <summary>
    /// Indicates whether <see cref="Data"/> contains text.
    /// </summary>
    /// <value>
    /// <c>true</c> if <see cref="Data"/> contains text, otherwise <c>false</c>.
    /// </value>
    public bool ContainsEmbeddedText => this.MimeTypeInfo.IsText || this.MimeTypeInfo.IsEmpty;

    /// <summary>
    /// Indicates whether <see cref="Data"/> contains binary data.
    /// </summary>
    /// <value>
    /// <c>true</c> if <see cref="Data"/> contains binary data, otherwise <c>false</c>.
    /// </value>
    public bool ContainsEmbeddedBytes => DataEncoding == DataEncoding.Base64 || !ContainsEmbeddedText;

    /// <summary>
    /// Indicates whether the instance contains no data.
    /// </summary>
    /// <value>
    /// <c>true</c> if the instance contains no data, otherwise <c>false</c>.
    /// </value>
    public bool IsEmpty => this.MimeTypeInfo.IsEmpty;

    /// <summary>
    /// Returns a <see cref="DataUrlInfo"/> instance, which is <see cref="Empty"/>.
    /// </summary>
    public static DataUrlInfo Empty => default;

}
