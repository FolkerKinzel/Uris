# FolkerKinzel.Uris
[![NuGet](https://img.shields.io/nuget/v/FolkerKinzel.Uris)](https://www.nuget.org/packages/FolkerKinzel.Uris/)
[![GitHub](https://img.shields.io/github/license/FolkerKinzel/Uris)](https://github.com/FolkerKinzel/Uris/blob/master/LICENSE)

.NET library that supports URIs and Internet Media Types ("MIME Types").

The library contains:
* `readonly struct MimeType`: Represents a MIME type ("Internet Media Type") according 
to RFC 2045, RFC 2046 and RFC 2184. The struct can be created automatically from a file type 
extension or parsed from a `String` or a `ReadOnlyMemory<Char>`. It is 
able to find an appropriate file type extension for its content. (Have a look at
 [MimeResourceCompiler](https://github.com/FolkerKinzel/Uris/blob/master/src/MimeResourceCompiler/Program.cs) that is part of the repository to
have a detailed explanation where the data comes from and how it is compiled.)
* `readonly struct DataUrl`: Represents a "data" URL (RFC 2397) that 
embeds data in-line in a URL. It enables you to retrieve this data and to find automatically an appropriate file type extension for it. A `DataUrl` can be created automatically from the data to embed. This can be a file, a byte array or a string. 

The library makes extensive use of ReadOnlySpan&lt;Char&gt; and ReadOnlyMemory&lt;Char&gt; to build and examine the 
content of such URIs without having to allocate a lot of temporary Strings.

Read the [Project Reference](https://github.com/FolkerKinzel/Uris/blob/master/ProjectReference/1.0.0-alpha1/FolkerKinzel.Uris.Reference.en.chm) for details.

> IMPORTANT: On some systems the content of the .CHM file is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox (if it is present) in the lower right corner of the General tab in the Properties dialog.
