using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;


#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        #region ToString

        /// <summary>
        /// Creates a complete <see cref="string"/> representation of the instance that includes the <see cref="Parameters"/>.
        /// </summary>
        /// <returns>A complete <see cref="string"/> representation of the instance that includes the <see cref="Parameters"/>.</returns>
        public override string ToString() => ToString(true);

        /// <summary>
        /// Creates a <see cref="string"/> representation of the instance and allows to determine, whether or not to include the
        /// <see cref="Parameters"/>.
        /// </summary>
        /// <param name="includeParameters">Pass <c>true</c> to include the <see cref="Parameters"/>; <c>false</c>, otherwise.</param>
        /// <returns>A <see cref="string"/> representation of the instance.</returns>
        public string ToString(bool includeParameters)
        {
            var sb = new StringBuilder(StringLength);
            _ = AppendTo(sb, includeParameters);
            return sb.ToString();
        }

        /// <summary>
        /// Appends a <see cref="string"/> representation of this instance to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/>.</param>
        /// <param name="includeParameters">Pass <c>true</c> to include the <see cref="Parameters"/>; <c>false</c>, otherwise.</param>
        /// <returns>A reference to <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        public StringBuilder AppendTo(StringBuilder builder, bool includeParameters = true)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (IsEmpty)
            {
                return builder;
            }

            _ = builder.EnsureCapacity(builder.Length + StringLength);
            int insertStartIndex = builder.Length;
            _ = builder.Append(TopLevelMediaType).Append('/').Append(SubType).ToLowerInvariant(insertStartIndex);

            if (includeParameters)
            {
                foreach (MimeTypeParameter parameter in Parameters)
                {
                    _ = builder.Append(';');
                    parameter.AppendTo(builder);
                }
            }

            return builder;
        }

        #endregion
    }
}
