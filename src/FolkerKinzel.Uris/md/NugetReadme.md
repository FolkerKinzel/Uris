[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)


### .NET library that supports working with URIs
[Project Reference and Release Notes](https://github.com/FolkerKinzel/Uris/releases/tag/v3.0.0-beta.1)

The library supports:
- The "data" URL scheme ([RFC 2397](https://datatracker.ietf.org/doc/html/rfc2397)) which allows to embed data directly inside of a URI. The static `DataUrl` class allows 
  - building such URIs from files, byte arrays or text that's to embed,
  - parsing "data" URL strings as `DataUrlInfo` structs in order to examine its content without having to allocate a lot of sub strings,
  - retrieving the embedded data from "data" URL strings,
  - retrieving an appropriate file type extension for the embedded data.

The library is designed to support performance and small heap allocation.

[See code examples at GitHub](https://github.com/FolkerKinzel/Uris)

[Version History](https://github.com/FolkerKinzel/Uris/releases)



