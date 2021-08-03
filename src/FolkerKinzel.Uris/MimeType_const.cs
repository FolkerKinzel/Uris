using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        #region const
        internal const int StringLength = 80;

        private const int SUB_TYPE_LENGTH_SHIFT = 8;
        private const int SUB_TYPE_START_SHIFT = 16;
        private const int MEDIA_TYPE_LENGTH_SHIFT = 24;

        #endregion
    }
}
