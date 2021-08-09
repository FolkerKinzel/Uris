[![GitHub](https://img.shields.io/github/license/FolkerKinzel/Uris)](https://github.com/FolkerKinzel/Uris/blob/master/LICENSE)

The current version of the package is a preview version. Updates of this preview version might contain breaking changes: Therefore it's not recommended to use these preview versions in productivity code.

The library contains:
* `readonly struct DataUrl`: Represents a "data" URL (RFC 2397) that embeds data in-line in a URL. It enables you to retrieve this data and to find automatically an appropriate file type extension for it. A `DataUrl` can be created automatically from the data to embed. This can be a file, a byte array or a string. 

The library makes extensive use of ReadOnlySpan&lt;Char&gt; and ReadOnlyMemory&lt;Char&gt; to build and examine the content of such URIs without having to allocate a lot of temporary Strings.

Read the [Project Reference](https://github.com/FolkerKinzel/Uris/blob/master/ProjectReference/1.0.0-alpha.1/FolkerKinzel.Uris.Reference.en.chm) for details.

> IMPORTANT: On some systems the content of the .CHM file is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox (if it is present) in the lower right corner of the General tab in the Properties dialog.


