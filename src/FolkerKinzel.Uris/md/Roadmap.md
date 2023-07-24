# FolkerKinzel.Uris
## Roadmap

### 2.0.0-beta.2
- [ ] Use `Convert.TryFromBase64Chars(ReadOnlySpan<char>, Span<byte>, int)`

### 2.0.0-beta.1
- [x] Dependency update
- [x] .NET 7 support
- [ ] Higher code coverage 

### 1.0.0-beta.2
- [x] Replace the method `DataUrlInfo.ParseBom` with `FolkerKinzel.Strings.ParseBom`
- [x] Split `DataUrl` in a static class `DataUrlBuilder` and a struct `DataUrlInfo`

### 1.0.0-beta.4
- [x] .NET 6.0 support.
- [x] Parses DataUrls that use the Base64Url format (RFC 4648 § 5).
