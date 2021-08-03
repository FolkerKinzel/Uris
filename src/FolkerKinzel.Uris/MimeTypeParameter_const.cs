using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeTypeParameter : IEquatable<MimeTypeParameter>, ICloneable
    {
        internal const int StringLength = 32;

        private const string CHARSET_KEY = "charset";
        private const string ASCII_CHARSET_VALUE = "us-ascii";
        
        
    }
}
