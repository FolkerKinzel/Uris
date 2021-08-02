using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrl : IEquatable<DataUrl>, ICloneable
    {
        /// <summary>
        /// Creates a hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(GetFileTypeExtension());

            if (TryGetEmbeddedText(out string? text))
            {
                hash.Add(text);
            }
            else if (TryGetEmbeddedBytes(out byte[]? bytes))
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Add(bytes[i]);
                }
            }
            return hash.ToHashCode();
        }
    }
}
