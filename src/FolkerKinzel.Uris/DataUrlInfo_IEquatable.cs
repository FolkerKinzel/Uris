﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrlInfo : IEquatable<DataUrlInfo>
    {
        /// <summary>
        /// Determines whether <paramref name="obj"/> is a <see cref="DataUrlInfo"/> structure whose
        /// value is equal to that of this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="DataUrlInfo"/> structure whose
        /// value is equal to that of this instance; <c>false</c>, otherwise.</returns>
        public override bool Equals(object? obj)
            => obj is DataUrlInfo other && Equals(in other);



        /// <summary>
        /// Determines whether the value of this instance is equal to the value of <paramref name="other"/>. 
        /// </summary>
        /// <param name="other">The <see cref="DataUrlInfo"/> instance to compare with.</param>
        /// <returns><c>true</c> if this the value of this instance is equal to that of <paramref name="other"/>; <c>false</c>, otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(DataUrlInfo other) => Equals(in other);

        /// <summary>
        /// Determines whether the value of this instance is equal to the value of <paramref name="other"/>. 
        /// </summary>
        /// <param name="other">The <see cref="DataUrlInfo"/> instance to compare with.</param>
        /// <returns><c>true</c> if this the value of this instance is equal to that of <paramref name="other"/>; <c>false</c>, otherwise.</returns>
        /// <remarks>This is the most performant overload of the Equals methods but unfortunately it's not CLS compliant.
        /// Use it if you can.</remarks>
        [CLSCompliant(false)]
        public bool Equals(in DataUrlInfo other)
            => this.IsEmpty || other.IsEmpty
                ? this.IsEmpty && other.IsEmpty
                : EqualsData(in other) && StringComparer.Ordinal.Equals(this.GetFileTypeExtension(), other.GetFileTypeExtension());

        #region private

        private bool EqualsData(in DataUrlInfo other)
            => this.ContainsEmbeddedText
                ? EqualsText(in other)
                : this.Encoding == ContentEncoding.Base64 && other.Encoding == ContentEncoding.Base64
                    ? this.Data.Equals(other.Data, StringComparison.Ordinal)
                    : EqualsBytes(in other);

        private bool EqualsText(in DataUrlInfo other)
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

        private bool EqualsBytes(in DataUrlInfo other)
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

    }
}
