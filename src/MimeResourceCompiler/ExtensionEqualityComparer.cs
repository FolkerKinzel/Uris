using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler
{
    public class ExtensionEqualityComparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry? x, Entry? y) => StringComparer.Ordinal.Equals(x?.Extension, y?.Extension);
        public int GetHashCode([DisallowNull] Entry obj) => obj.Extension.GetHashCode();
    }
}
