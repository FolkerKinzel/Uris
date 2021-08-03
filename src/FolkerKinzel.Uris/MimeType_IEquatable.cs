using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        #region IEquatable
        /// <summary>
        /// Determines whether the value of this instance is equal to the value of <paramref name="other"/>. The <see cref="Parameters"/>
        /// are taken into account.
        /// </summary>
        /// <param name="other">The <see cref="MimeType"/> instance to compare with.</param>
        /// <returns><c>true</c> if this the value of this instance is equal to that of <paramref name="other"/>; <c>false</c>, otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(MimeType other) => Equals(in other, false);

        /// <summary>
        /// Determines whether the value of this instance is equal to the value of <paramref name="other"/>. The <see cref="Parameters"/>
        /// are taken into account.
        /// </summary>
        /// <param name="other">The <see cref="MimeType"/> instance to compare with.</param>
        /// <returns><c>true</c> if this the value of this instance is equal to that of <paramref name="other"/>; <c>false</c>, otherwise.</returns>
        /// <remarks>This is the most performant overload of the Equals methods but unfortunately it's not CLS compliant.
        /// Use it if you can.</remarks>
        [CLSCompliant(false)]
        public bool Equals(in MimeType other) => Equals(in other, false);

        ///// <summary>
        ///// Determines whether this instance is equal to <paramref name="other"/> and allows to specify
        ///// whether or not the <see cref="Parameters"/> are taken into account.
        ///// </summary>
        ///// <param name="other">The <see cref="MimeType"/> instance to compare with.</param>
        ///// <param name="ignoreParameters">Pass <c>false</c> to take the <see cref="Parameters"/> into account;
        ///// <c>true</c>, otherwise.</param>
        ///// <returns><c>true</c> if this  instance is equal to <paramref name="other"/>; false, otherwise.</returns>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public bool Equals(MimeType other, bool ignoreParameters) => Equals(in other, ignoreParameters);

        /// <summary>
        /// Determines whether this instance is equal to <paramref name="other"/> and allows to specify
        /// whether or not the <see cref="Parameters"/> are taken into account.
        /// </summary>
        /// <param name="other">The <see cref="MimeType"/> instance to compare with.</param>
        /// <param name="ignoreParameters">Pass <c>false</c> to take the <see cref="Parameters"/> into account;
        /// <c>true</c>, otherwise.</param>
        /// <returns><c>true</c> if this  instance is equal to <paramref name="other"/>; false, otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        //[CLSCompliant(false)]
        public bool Equals(in MimeType other, bool ignoreParameters)
        {
            if (!MediaType.Equals(other.MediaType, StringComparison.OrdinalIgnoreCase) ||
               !SubType.Equals(other.SubType, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (ignoreParameters)
            {
                return true;
            }

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            IOrderedEnumerable<MimeTypeParameter> thisParameters;
            IOrderedEnumerable<MimeTypeParameter> otherParameters;

            if (IsText)
            {
                thisParameters = Parameters.SkipWhile(UsAsciiPredicate).OrderBy(KeySelector, comparer);
                otherParameters = other.Parameters.SkipWhile(UsAsciiPredicate).OrderBy(KeySelector, comparer);
            }
            else
            {
                thisParameters = Parameters.OrderBy(KeySelector, comparer);
                otherParameters = other.Parameters.OrderBy(KeySelector, comparer);
            }

            return thisParameters.SequenceEqual(otherParameters);
        }

        /// <summary>
        /// Determines whether <paramref name="obj"/> is a <see cref="MimeType"/> structure whose
        /// value is equal to that of this instance. The <see cref="Parameters"/>
        /// are taken into account.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="MimeType"/> structure whose
        /// value is equal to that of this instance; <c>false</c>, otherwise.</returns>
        public override bool Equals(object? obj) => obj is MimeType type && Equals(in type, false);

        #region private
        private static bool UsAsciiPredicate(MimeTypeParameter x) => x.IsAsciiCharsetParameter();

        private static string KeySelector(MimeTypeParameter parameter) => parameter.Key.ToString();
        #endregion
        #endregion
    }
}
