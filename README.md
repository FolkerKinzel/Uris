# FolkerKinzel.Uris
[![NuGet](https://img.shields.io/nuget/v/FolkerKinzel.Uris)](https://www.nuget.org/packages/FolkerKinzel.Uris/)
[![GitHub](https://img.shields.io/github/license/FolkerKinzel/Uris)](https://github.com/FolkerKinzel/Uris/blob/master/LICENSE)

.NET library that supports URIs and Internet Media Types ("MIME Types").

The library contains:
* `readonly struct MimeType`: Represents a MIME type ("Internet Media Type") according 
to RFC 2045 and RFC 2046. The struct can be created automatically from a file type extension or parsed from a String or a `ReadOnlyMemory<Char>`.
* `readonly struct DataUrl`: Represents a "data" URL (RFC 2397) that embeds data in-line in a URL. It enables you to retrieve this data and to find automatically an appropriate file type extension for it. A `DataUrl` can be created automatically from the data to embed. This can be a file, a byte array or a string. 

.

[Download Project Reference](https://github.com/FolkerKinzel/Uris/blob/master/ProjectReference/1.0.0-alpha1/FolkerKinzel.Uris.Reference.en.chm)


> IMPORTANT: On some systems the content of the .CHM file is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox (if it is present) in the lower right corner of the General tab in the Properties dialog.
