# FolkerKinzel.Uris
## Roadmap
### 5.1.0
- [ ] Cleanup: Remove deprecated symbols
- [ ] Implement `string DataUrl.FromBytes(IEnumerable<byte>?, string?, DataEncoding)`
- [ ] Implement `string DataUrl.FromBytes(ReadOnlySpan<byte>, string?, DataEncoding)`
- [ ] Implement `string DataUrl.FromBytes(IEnumerable<byte>?, in MimeTypeInfo, DataEncoding)`
- [ ] Implement `string DataUrl.FromBytes(ReadOnlySpan<byte>, in MimeTypeInfo, DataEncoding)`
- [ ] Implement `StringBuilder DataUrl.AppendEmbeddedBytesTo(StringBuilder, IEnumerable<byte>?, string?, DataEncoding)`
- [ ] Implement `StringBuilder DataUrl.AppendEmbeddedBytesTo(StringBuilder, ReadOnlySpan<byte>, string?, DataEncoding)`
- [ ] Implement `StringBuilder DataUrl.AppendEmbeddedBytesTo(StringBuilder, IEnumerable<byte>?, in MimeTypeInfo, DataEncoding)`
- [ ] Implement `StringBuilder DataUrl.AppendEmbeddedBytesTo(StringBuilder, ReadOnlySpan<byte>, in MimeTypeInfo, DataEncoding)`

### 2.0.0-beta.1
- [x] Dependency update
- [x] .NET 7 support
- [x] Higher code coverage 

### 1.0.0-beta.2
- [x] Replace the method `DataUrlInfo.ParseBom` with `FolkerKinzel.Strings.ParseBom`
- [x] Split `DataUrl` in a static class `DataUrlBuilder` and a struct `DataUrlInfo`

### 1.0.0-beta.4
- [x] .NET 6.0 support.
- [x] Parses DataUrls that use the Base64Url format (RFC 4648 § 5).
