[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)


### .NET library that supports working with URIs
[Project Reference and Release Notes](https://github.com/FolkerKinzel/Uris/releases/tag/v2.0.0-beta.1)

The library supports:
- The "data" URL scheme ([RFC 2397](https://datatracker.ietf.org/doc/html/rfc2397)):
  - The static `DataUrlBuilder` class: Provides functionality to build a "data" URL that embeds data in-line in a URL-string. DataUrlBuilder creates a "data" URL-string from the data to embed. This can be either a file or a byte array or a string. 
  - The `DataUrlInfo` structure allows to retrieve the data from a "data" URL-string and to find automatically an appropriate file type extension for it.

The library is designed to support performance and small heap allocation.

[See code examples at GitHub](https://github.com/FolkerKinzel/Uris)

[Version History](https://github.com/FolkerKinzel/Uris/releases)



