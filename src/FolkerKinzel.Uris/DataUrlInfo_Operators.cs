using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrlInfo : IEquatable<DataUrlInfo>, ICloneable
    {
        #region Operators

        /// <summary>
        /// Returns a value that indicates whether the values of two specified <see cref="DataUrlInfo"/> instances are equal.
        /// </summary>
        /// <param name="dataUrl1">The first <see cref="DataUrlInfo"/> to compare.</param>
        /// <param name="dataUrl2">The second <see cref="DataUrlInfo"/> to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="dataUrl1"/> and <paramref name="dataUrl2"/> are equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(DataUrlInfo dataUrl1, DataUrlInfo dataUrl2) => dataUrl1.Equals(in dataUrl2);

        /// <summary>
        /// Returns a value that indicates whether the values of two specified <see cref="DataUrlInfo"/> instances are not equal.
        /// </summary>
        /// <param name="obj1">The first <see cref="DataUrlInfo"/> to compare.</param>
        /// <param name="dataUrl2">The second <see cref="DataUrlInfo"/> to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="obj1"/> and <paramref name="dataUrl2"/> are not equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(DataUrlInfo obj1, DataUrlInfo dataUrl2) => !obj1.Equals(in dataUrl2);

        ///// <summary>
        ///// Returns a value that indicates whether the values of two specified <see cref="DataUrl"/> instances are equal.
        ///// </summary>
        ///// <param name="obj1">The first <see cref="DataUrl"/> to compare.</param>
        ///// <param name="obj2">The second <see cref="DataUrl"/> to compare.</param>
        ///// <returns><c>true</c> if the values of <paramref name="obj1"/> and <paramref name="obj2"/> are equal;
        ///// otherwise, <c>false</c>.</returns>
        //public static bool operator ==(DataUrl obj1, object? obj2) => obj1.Equals(obj2);

        ///// <summary>
        ///// Returns a value that indicates whether the values of two specified <see cref="DataUrl"/> instances are not equal.
        ///// </summary>
        ///// <param name="obj1">The first <see cref="DataUrl"/> to compare.</param>
        ///// <param name="obj2">The second <see cref="DataUrl"/> to compare.</param>
        ///// <returns><c>true</c> if the values of <paramref name="obj1"/> and <paramref name="obj2"/> are not equal;
        ///// otherwise, <c>false</c>.</returns>
        //public static bool operator !=(DataUrl obj1, object? obj2) => !(obj1 == obj2);

        #endregion


    }
}
