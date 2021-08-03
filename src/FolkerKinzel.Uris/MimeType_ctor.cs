using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FolkerKinzel.Uris.Intls;
using FolkerKinzel.Uris.Properties;

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    /// <summary>
    /// Represents a MIME type ("Internet Media Type") according to RFC 2045, RFC 2046 and RFC 2184.
    /// </summary>
    /// <remarks>
    /// <note type="tip">
    /// <para>
    /// <see cref="MimeType"/> is a quite large structure. Pass it to other methods by reference (in, ref or out parameters in C#)!
    /// </para>
    /// <para>
    /// If you intend to hold a <see cref="MimeType"/> for a long time in memory and if this <see cref="MimeType"/> is parsed
    /// from a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from a very long <see cref="string"/>, 
    /// keep in mind, that the <see cref="MimeType"/> holds a reference to that <see cref="string"/>. Consider in this case to make
    /// a copy of the <see cref="MimeType"/> structure with <see cref="MimeType.Clone"/>: The copy is built on a separate <see cref="string"/>,
    /// which is case-normalized and only as long as needed.
    /// </para>
    /// </note>
    /// </remarks>
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        private MimeType(in ReadOnlyMemory<char> mimeTypeString, int idx)
        {
            this._mimeTypeString = mimeTypeString;
            this._idx = idx;
        }
    }
}
