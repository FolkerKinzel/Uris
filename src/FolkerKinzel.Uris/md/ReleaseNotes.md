- Dependency update
- New method overloads:
```csharp
string DataUrl.FromBytes(IEnumerable<byte>?, string?, DataEncoding);
string DataUrl.FromBytes(ReadOnlySpan<byte>, string?, DataEncoding);
string DataUrl.FromBytes(IEnumerable<byte>?, in MimeTypeInfo, DataEncoding);
string DataUrl.FromBytes(ReadOnlySpan<byte>, in MimeTypeInfo, DataEncoding);
StringBuilder DataUrl.AppendEmbeddedBytesTo(StringBuilder, IEnumerable<byte>?, string?, DataEncoding);
StringBuilder DataUrl.AppendEmbeddedBytesTo(StringBuilder, ReadOnlySpan<byte>, string?, DataEncoding);
StringBuilder DataUrl.AppendEmbeddedBytesTo(StringBuilder, IEnumerable<byte>?, in MimeTypeInfo, DataEncoding);
StringBuilder DataUrl.AppendEmbeddedBytesTo(StringBuilder, ReadOnlySpan<byte>, in MimeTypeInfo, DataEncoding);
```

.

>**Project reference:** On some systems, the content of the CHM file in the Assets is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox - if it is present - in the lower right corner of the General tab in the Properties dialog.