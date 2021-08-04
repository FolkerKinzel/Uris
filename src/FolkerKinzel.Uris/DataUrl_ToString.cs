using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;

#if NET461 || NETSTANDARD2_0
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrl : IEquatable<DataUrl>, ICloneable
    {
        /// <summary>
        /// Creates a "data" URL (RFC 2397) representation of the instance.
        /// </summary>
        /// <returns>A "data" URL (RFC 2397) representation of the instance.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder(ComputeCapacity());
            return AppendTo(builder).ToString();
        }

        /// <summary>
        /// Appends a <see cref="string"/> representation of this instance to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/>.</param>
        /// <returns>A reference to <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        public StringBuilder AppendTo(StringBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (Encoding == ContentEncoding.Base64 || IsEmpty)
            {
                _ = builder.EnsureCapacity(ComputeCapacity());
                _ = builder.Append(PROTOCOL).AppendMediaType(MimeType).Append(BASE64).Append(',').Append(Data);
            }
            else if(TryGetEmbeddedText(out string? text))
            {
                string urlString = DataUrl.FromText(text);
                _ = builder.Append(urlString);
            }
            else // URL encoded bytes
            {
                _ = TryGetEmbeddedBytes(out byte[]? bytes);
                string urlString = DataUrl.FromBytes(bytes, in _mimeType);
                _ = builder.Append(urlString);
            }

            return builder;
        }

        #region private

        private int ComputeCapacity()
        {
            int capacity = PROTOCOL.Length + MimeTypes.MimeType.StringLength + Data.Length + 1;

            if (Encoding == ContentEncoding.Base64)
            {
                capacity += BASE64.Length;
            }

            return capacity;
        }

        #endregion
    }
}
