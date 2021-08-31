[![GitHub](https://img.shields.io/github/license/FolkerKinzel/Uris)](https://github.com/FolkerKinzel/Uris/blob/master/LICENSE)

The library contains:
* `readonly struct DataUrl`: Represents a "data" URL (RFC 2397) that embeds data in-line in a URL. It enables you to retrieve this data and to find automatically an appropriate file type extension for it. A `DataUrl` can be created automatically from the data to embed. This can be a file, a byte array or a string. 

The library makes extensive use of ReadOnlySpan&lt;Char&gt; and ReadOnlyMemory&lt;Char&gt; to build and examine the content of such URIs without having to allocate a lot of temporary Strings.

Read the [Project Reference](https://github.com/FolkerKinzel/Uris/blob/master/ProjectReference/1.0.0-beta.1/FolkerKinzel.Uris.Reference.en.chm) for details.

> IMPORTANT: On some systems the content of the .CHM file is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox (if it is present) in the lower right corner of the General tab in the Properties dialog.

.
### Example:
Creating and parsing a "data" URL:
```csharp
using System;
using System.Diagnostics;
using System.IO;
using FolkerKinzel.Uris;

namespace Examples
{
    public static class DataUrlExample
    {
        public static void Example()
        {
            string fotoFilePath = CreatePhotoFile();
            string dataUrl = DataUrl.FromFile(fotoFilePath);
            File.Delete(fotoFilePath);

            // Uncomment this, to show the content of the
            // "data" URL in the Microsoft Edge browser.
            // (Make shure to have this browser installed.):
            // ShowPictureInMicrosoftEdge(dataUrl);

            Console.WriteLine(dataUrl);
            Console.WriteLine();

            // Parse the content of the "data" URL:
            var info = DataUrlInfo.Parse(dataUrl);

            Console.WriteLine($"Contains Bytes: {info.ContainsEmbeddedBytes}");
            Console.WriteLine($"Contains Text:  {info.ContainsEmbeddedText}");
            Console.WriteLine($"MIME Type:      {info.MimeType}");
            Console.WriteLine($"File Type Ext.: {info.GetFileTypeExtension()}");
            Console.WriteLine($"Data Encoding:  {info.DataEncoding}");

            if (info.TryGetEmbeddedBytes(out byte[]? bytes))
            {
                Console.WriteLine($"Data Length:    {bytes.Length} Bytes");
            }
        }

        private static string CreatePhotoFile()
        {
            string url = "data:image/jpeg;base64,/9j/4QAYRXhpZgAA ~ 0JFFIH0+uvTs//Z";

            var info = DataUrlInfo.Parse(url);
            string path = Path.Combine(Directory.GetCurrentDirectory(),
                                       Guid.NewGuid().ToString() + 
                                       info.GetFileTypeExtension());

            if (info.TryGetEmbeddedBytes(out byte[]? bytes))
            {
                File.WriteAllBytes(path, bytes);
            }

            return path;
        }

        private static void ShowPictureInMicrosoftEdge(string dataUrl)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = "msedge";
            process.StartInfo.Arguments = dataUrl;
            _ = process.Start();
        }
    }
}

/*
Console Output:

data:image/jpeg;base64,/9j/4QAYRXhpZgAA ~ 0JFFIH0+uvTs//Z

Contains Bytes: True
Contains Text:  False
MIME Type:      image/jpeg
File Type Ext.: .jpg
Data Encoding:  Base64
Data Length:    2472 Bytes
 */

```
.
- [Version History](https://github.com/FolkerKinzel/Uris/releases)

