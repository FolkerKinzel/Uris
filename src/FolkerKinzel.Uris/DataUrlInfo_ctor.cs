﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes;
using FolkerKinzel.Uris.Extensions;
using FolkerKinzel.Uris.Intls;
using FolkerKinzel.Uris.Properties;

#if NET461 || NETSTANDARD2_0
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    /// <summary>
    /// Provides the information stored in a "data" URL (RFC 2397).
    /// </summary>
    /// <remarks>
    /// <note type="tip">
    /// <para>
    /// <see cref="DataUrlInfo"/> is a quite large structure. Pass it to other methods by reference (in, ref or out parameters in C#)!
    /// </para>
    /// <para>
    /// If you intend to hold a <see cref="DataUrlInfo"/> for a long time in memory and if this <see cref="DataUrlInfo"/> is parsed
    /// from a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from a very long <see cref="string"/>, 
    /// keep in mind, that the <see cref="DataUrlInfo"/> holds a reference to that <see cref="string"/>. Consider in this case to make
    /// a copy of the <see cref="DataUrlInfo"/> structure with <see cref="DataUrlInfo.Clone"/>: The copy is built on a separate <see cref="string"/>,
    /// which is case-normalized and only as long as needed.
    /// </para>
    /// </note>
    /// </remarks>
    public readonly partial struct DataUrlInfo
    {
        private DataUrlInfo(in MimeType mediaType, DataEncoding dataEncoding, in ReadOnlyMemory<char> embeddedData)
        {
            _mimeType = mediaType;
            DataEncoding = dataEncoding;
            _embeddedData = embeddedData;
        }
    }
}
