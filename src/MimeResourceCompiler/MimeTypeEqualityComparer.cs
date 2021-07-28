using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler
{
    public class MimeTypeEqualityComparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry? x, Entry? y) => StringComparer.Ordinal.Equals(x?.MimeType, y?.MimeType);
        public int GetHashCode([DisallowNull] Entry obj) => obj.MimeType.GetHashCode();
    }
}
