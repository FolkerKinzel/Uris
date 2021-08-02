using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrl : IEquatable<DataUrl>, ICloneable
    {
        #region const
        internal const string PROTOCOL = "data:";

        private const string BASE64 = ";base64";
        private const string DEFAULT_MEDIA_TYPE = "text/plain";

        #endregion
    }
}
