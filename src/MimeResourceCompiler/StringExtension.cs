using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler
{
    public static class StringExtension
    {
        public static string PrepareMimeType(this string mimeType) => mimeType.ToLowerInvariant();

        public static string PrepareFileTypeExtension(this string fileTypeExtension)
            => fileTypeExtension.Replace(".", null, StringComparison.Ordinal).ToLowerInvariant();

    }
}
