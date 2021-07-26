# FolkerKinzel.Uris
[![NuGet](https://img.shields.io/nuget/v/FolkerKinzel.Uris)](https://www.nuget.org/packages/FolkerKinzel.Uris/)
[![GitHub](https://img.shields.io/github/license/FolkerKinzel/Uris)](https://github.com/FolkerKinzel/Uris/blob/master/LICENSE)

.NET library that supports URIs and Internet Media Types ("MIME Types").

The library contains:
* `readonly struct MimeType`: Represents a MIME type ("Internet Media Type") according 
to RFC 2045 and RFC 2046. The struct can be created automatically from a file type extension or parsed from a String or a `ReadOnlyMemory<char>`.
* `readonly struct DataUrl`: Represents a "data" URL (RFC 2397) that embeds data in-line in a URL. It enables you to retrieve this data and to find automatically an appropriate file type extension for it. A `DataUrl` can be created automatically from the data to embed. This can be a file, a byte array or a string. 


