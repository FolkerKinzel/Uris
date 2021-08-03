using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        #region GetHashCode

        /// <summary>
        /// Creates a hash code for this instance, which takes <see cref="Parameters"/> into account.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => GetHashCode(false);

        /// <summary>
        /// Creates a hash code for this instance and allows to specify whether or not
        /// the <see cref="Parameters"/> are taken into account.
        /// </summary>
        /// <param name="ignoreParameters">Pass <c>false</c> to take the <see cref="Parameters"/> into account; <c>true</c>, otherwise.</param>
        /// <returns>The hash code.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public int GetHashCode(bool ignoreParameters)
        {
            var hash = new HashCode();

            ReadOnlySpan<char> mediaTypeSpan = MediaType;
            for (int i = 0; i < mediaTypeSpan.Length; i++)
            {
                hash.Add(char.ToLowerInvariant(mediaTypeSpan[i]));
            }

            ReadOnlySpan<char> subTypeSpan = SubType;
            for (int j = 0; j < subTypeSpan.Length; j++)
            {
                hash.Add(char.ToLowerInvariant(subTypeSpan[j]));
            }

            if (ignoreParameters)
            {
                return hash.ToHashCode();
            }

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            IOrderedEnumerable<MimeTypeParameter> thisParameters =
                IsText
                ? Parameters.SkipWhile(UsAsciiPredicate).OrderBy(KeySelector, comparer)
                : Parameters.OrderBy(KeySelector, comparer);

            foreach (MimeTypeParameter parameter in thisParameters)
            {
                hash.Add(parameter);
            }

            return hash.ToHashCode();
        }

        #endregion

    }
}
