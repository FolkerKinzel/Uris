[![GitHub](https://img.shields.io/github/license/FolkerKinzel/Uris)](https://github.com/FolkerKinzel/Uris/blob/master/LICENSE)

The library supports:
* The "data" URL scheme (RFC 2397):
  * The static `DataUrlBuilder` class: Provides functionality to build a "data" URL that embeds data in-line in a URL. A "data" URL can be created automatically from the data to embed. This can be a file, a byte array or a string. 
  * The `DataUrlInfo` structure allows to retrieve the data from a "data" URL and to find automatically an appropriate file type extension for it.

The library makes use of ReadOnlySpan&lt;Char&gt; and ReadOnlyMemory&lt;Char&gt; in order to build and examine the 
content of such URIs without having to allocate a lot of temporary Strings.

Read the [Project Reference](https://github.com/FolkerKinzel/Uris/blob/master/ProjectReference/1.0.0-beta.4/FolkerKinzel.Uris.Reference.en.chm) for details.

> IMPORTANT: On some systems the content of the .CHM file is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox (if it is present) in the lower right corner of the General tab in the Properties dialog.

.
- [Version History](https://github.com/FolkerKinzel/Uris/releases)

