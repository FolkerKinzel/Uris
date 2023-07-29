﻿using System.Runtime.InteropServices;

namespace FolkerKinzel.Uris;

#pragma warning disable CA1303 // Literale nicht als lokalisierte Parameter übergeben

[StructLayout(LayoutKind.Auto)]
public readonly partial struct DataUrlInfo
{
    private const int MIME_TYPE_LENGTH_SHIFT = 1;
    private const int DATA_ENCODING_MAX_VALUE = 1;
    private const int COMMA_LENGTH = 1;
    private const ushort MIME_TYPE_LENGTH_MAX_VALUE = 0b0111_1111_1111_1111;

    private readonly ReadOnlyMemory<char> _embeddedData;
    private readonly ushort _idx;

    private int MimeTypeLength => _idx >> 1;

    private int EmbeddedDataStartIndex => IsEmpty ? 0 
                                                  : MimeTypeLength
                                                    + (DataEncoding == DataEncoding.Base64 ? DataUrl.Base64.Length : 0)
                                                    + COMMA_LENGTH;

    /// <summary>
    /// Information about the internet media type of the embedded data.
    /// </summary>
    public ReadOnlyMemory<char> MimeType => MimeTypeLength == 0 ? DataUrl.DefaultMediaType.AsMemory() : _embeddedData.Slice(0, MimeTypeLength);

    /// <summary>
    /// The encoding of the data in <see cref="Data"/>.
    /// </summary>
    public DataEncoding DataEncoding => (DataEncoding)(_idx & DATA_ENCODING_MAX_VALUE);

    /// <summary>
    /// The part of the "data" URL, which contains the embedded data.
    /// </summary>
    public ReadOnlySpan<char> Data => _embeddedData.Span.Slice(EmbeddedDataStartIndex);

    /// <summary>
    /// Indicates whether <see cref="Data"/> contains text.
    /// </summary>
    /// <value>
    /// <c>true</c> if <see cref="Data"/> contains text, otherwise <c>false</c>.
    /// </value>
    public bool ContainsEmbeddedText => MimeTypeLength == 0 || _embeddedData.Span.StartsWith("text/".AsSpan(), StringComparison.OrdinalIgnoreCase);

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
    public bool IsEmpty => _embeddedData.IsEmpty;

    /// <summary>
    /// Returns a <see cref="DataUrlInfo"/> instance, which is <see cref="Empty"/>.
    /// </summary>
    public static DataUrlInfo Empty => default;

}

#pragma warning restore CA1303 // Literale nicht als lokalisierte Parameter übergeben
