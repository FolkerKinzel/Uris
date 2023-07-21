[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)


### .NET library that supports working with URIs
[Project Reference and Release Notes](https://github.com/FolkerKinzel/Uris/releases/tag/v1.0.0-beta.5)

The library supports:
- The "data" URL scheme (RFC 2397):
  - The static `DataUrlBuilder` class: Provides functionality to build a "data" URL that embeds data in-line in a URL. A "data" URL can be created automatically from the data to embed. This can be a file, a byte array or a string. 
  - The `DataUrlInfo` structure allows to retrieve the data from a "data" URL and to find automatically an appropriate file type extension for it.

The library is designed to support performance and small heap allocation.

[See code examples at GitHub](https://github.com/FolkerKinzel/Uris)

[Version History](https://github.com/FolkerKinzel/Uris/releases)



