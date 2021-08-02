using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrl : IEquatable<DataUrl>, ICloneable
    {
        #region IEquatable

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

        /// <summary>
        /// Determines whether <paramref name="obj"/> is a <see cref="DataUrl"/> structure whose
        /// value is equal to that of this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="DataUrl"/> structure whose
        /// value is equal to that of this instance; <c>false</c>, otherwise.</returns>
        public override bool Equals(object? obj) => obj is DataUrl other && Equals(in other);

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

        /// <summary>
        /// Determines whether the value of this instance is equal to the value of <paramref name="other"/>. 
        /// </summary>
        /// <param name="other">The <see cref="DataUrl"/> instance to compare with.</param>
        /// <returns><c>true</c> if this the value of this instance is equal to that of <paramref name="other"/>; <c>false</c>, otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(DataUrl other) => Equals(in other);

        /// <summary>
        /// Determines whether the value of this instance is equal to the value of <paramref name="other"/>. 
        /// </summary>
        /// <param name="other">The <see cref="DataUrl"/> instance to compare with.</param>
        /// <returns><c>true</c> if this the value of this instance is equal to that of <paramref name="other"/>; <c>false</c>, otherwise.</returns>
        /// <remarks>This is the most performant overload of the Equals methods but unfortunately it's not CLS compliant.
        /// Use it if you can.</remarks>
        [CLSCompliant(false)]
        public bool Equals(in DataUrl other)
            => this.IsEmpty || other.IsEmpty
                ? this.IsEmpty && other.IsEmpty
                : EqualsData(in other) && StringComparer.Ordinal.Equals(this.GetFileTypeExtension(), other.GetFileTypeExtension());

        #region private

        private bool EqualsData(in DataUrl other)
            => this.ContainsText
                ? EqualsText(in other)
                : this.DataEncoding == DataEncoding.Base64 && other.DataEncoding == DataEncoding.Base64
                    ? this.EmbeddedData.Equals(other.EmbeddedData, StringComparison.Ordinal)
                    : EqualsBytes(in other);

        private bool EqualsText(in DataUrl other)
        {
            if (other.TryGetEmbeddedText(out string? otherText))
            {
                if (this.TryGetEmbeddedText(out string? thisText))
                {
                    return StringComparer.Ordinal.Equals(thisText, otherText);
                }
            }

            return false;
        }

        private bool EqualsBytes(in DataUrl other)
        {
            if (other.TryGetEmbeddedBytes(out byte[]? otherBytes))
            {
                if (this.TryGetEmbeddedBytes(out byte[]? thisBytes))
                {
                    return thisBytes.SequenceEqual(otherBytes);
                }
            }

            return false;
        }

        #endregion
        #endregion
        

    }
}
