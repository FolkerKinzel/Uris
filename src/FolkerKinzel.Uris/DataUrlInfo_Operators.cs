using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrlInfo
    {
        #region Operators

        /// <summary>
        /// Returns a value that indicates whether the values of two specified <see cref="DataUrlInfo"/> instances are equal.
        /// </summary>
        /// <param name="value1">The first <see cref="DataUrlInfo"/> to compare.</param>
        /// <param name="value2">The second <see cref="DataUrlInfo"/> to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="value1"/> and <paramref name="value2"/> are equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(DataUrlInfo value1, DataUrlInfo value2) => value1.Equals(in value2);

        /// <summary>
        /// Returns a value that indicates whether the values of two specified <see cref="DataUrlInfo"/> instances are not equal.
        /// </summary>
        /// <param name="value1">The first <see cref="DataUrlInfo"/> to compare.</param>
        /// <param name="value2">The second <see cref="DataUrlInfo"/> to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="value1"/> and <paramref name="value2"/> are not equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(DataUrlInfo value1, DataUrlInfo value2) => !value1.Equals(in value2);

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
