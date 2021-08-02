using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrl : IEquatable<DataUrl>, ICloneable
    {
        #region ICloneable

        /// <inheritdoc/>
        /// <remarks>
        /// If you intend to hold a <see cref="DataUrl"/> for a long time in memory and if this <see cref="DataUrl"/> is parsed
        /// from a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from a very long <see cref="string"/>, 
        /// keep in mind, that the <see cref="DataUrl"/> holds a reference to that <see cref="string"/>. Consider in this case to make
        /// a copy of the <see cref="DataUrl"/> structure: The copy is built on a separate <see cref="string"/>,
        /// which is case-normalized and only as long as needed.
        /// <note type="tip">
        /// Use the instance method <see cref="DataUrl.Clone"/> if you can to avoid the costs of boxing.
        /// </note>
        /// </remarks>
        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Creates a new <see cref="DataUrl"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="DataUrl"/>, which is a copy of this instance.</returns>
        /// <remarks>
        /// The copy is built on a separate <see cref="string"/>,
        /// which is case-normalized and only as long as needed.
        /// </remarks>
        public DataUrl Clone()
        {
            if (IsEmpty)
            {
                return default;
            }

            ReadOnlyMemory<char> memory = ToString().AsMemory();
            _ = TryParse(in memory, out DataUrl dataUrl);

            return dataUrl;
        }

        #endregion


    }
}
