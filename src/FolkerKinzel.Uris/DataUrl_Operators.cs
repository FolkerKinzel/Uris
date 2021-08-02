using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrl : IEquatable<DataUrl>, ICloneable
    {
        #region Operators

        /// <summary>
        /// Returns a value that indicates whether the values of two specified <see cref="DataUrl"/> instances are equal.
        /// </summary>
        /// <param name="dataUrl1">The first <see cref="DataUrl"/> to compare.</param>
        /// <param name="dataUrl2">The second <see cref="DataUrl"/> to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="dataUrl1"/> and <paramref name="dataUrl2"/> are equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(DataUrl dataUrl1, DataUrl dataUrl2) => dataUrl1.Equals(in dataUrl2);

        /// <summary>
        /// Returns a value that indicates whether the values of two specified <see cref="DataUrl"/> instances are not equal.
        /// </summary>
        /// <param name="dataUrl1">The first <see cref="DataUrl"/> to compare.</param>
        /// <param name="dataUrl2">The second <see cref="DataUrl"/> to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="dataUrl1"/> and <paramref name="dataUrl2"/> are not equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(DataUrl dataUrl1, DataUrl dataUrl2) => !dataUrl1.Equals(in dataUrl2);

        #endregion


    }
}
