using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeTypeParameter : IEquatable<MimeTypeParameter>, ICloneable
    {
        #region ICloneable
        /// <inheritdoc/>
        /// <remarks>
        /// If you intend to hold a <see cref="MimeTypeParameter"/> for a long time in memory and if this <see cref="MimeTypeParameter"/> is parsed
        /// from a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from a very long <see cref="string"/>, 
        /// keep in mind, that the <see cref="MimeTypeParameter"/> holds a reference to that <see cref="string"/>. Consider in this case to make
        /// a copy of the <see cref="MimeTypeParameter"/> structure: The copy is built on a separate <see cref="string"/>,
        /// which is case-normalized and only as long as needed.
        /// <note type="tip">
        /// Use the instance method <see cref="MimeTypeParameter.Clone"/> if you can to avoid the costs of boxing.
        /// </note>
        /// </remarks>
        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Creates a new <see cref="MimeTypeParameter"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="MimeTypeParameter"/>, which is a copy of this instance.</returns>
        /// <remarks>
        /// The copy is built on a separate <see cref="string"/>,
        /// which is case-normalized and only as long as needed.
        /// </remarks>
        public MimeTypeParameter Clone()
        {
            if (IsEmpty)
            {
                return default;
            }

            ReadOnlyMemory<char> memory = ToString().AsMemory();
            _ = TryParse(ref memory, out MimeTypeParameter mimeTypeParameter, out bool _);
            return mimeTypeParameter;
        }

        #endregion


    }
}
