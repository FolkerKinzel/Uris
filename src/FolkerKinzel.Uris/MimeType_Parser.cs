using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Strings.Polyfills;
using FolkerKinzel.Uris.Intls;
using FolkerKinzel.Uris.Properties;

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        #region Parser

        /// <summary>
        /// Parses a <see cref="string"/> as <see cref="MimeType"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <returns>The <see cref="MimeType"/> instance, which <paramref name="value"/> represents.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="MimeType"/>.</exception>
        public static MimeType Parse(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(value);
            }

            ReadOnlyMemory<char> memory = value.AsMemory();

            return TryParse(ref memory, out MimeType mediaType)
                    ? mediaType
                    : throw new ArgumentException(string.Format(Res.InvalidMimeType, nameof(value)), nameof(value));
        }


        /// <summary>
        /// Tries to parse a <see cref="string"/> as <see cref="MimeType"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <param name="mimeType">When the method successfully returns, the parameter contains the
        /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string? value, out MimeType mimeType)
        {
            if (value is null)
            {
                mimeType = default;
                return false;
            }

            ReadOnlyMemory<char> memory = value.AsMemory();

            return TryParse(ref memory, out mimeType);
        }

        /// <summary>
        /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="MimeType"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse. The method might replace the 
        /// passed instance with a smaller one. Make a copy of the argument in the calling method if this is 
        /// not desirable.</param>
        /// <param name="mimeType">When the method successfully returns, the parameter contains the
        /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
        public static bool TryParse(ref ReadOnlyMemory<char> value, out MimeType mimeType)
        {
            value = value.TrimStart();
            ReadOnlySpan<char> span = value.Span;

            int parameterStartIndex = span.IndexOf(';');

            if(parameterStartIndex > byte.MaxValue) // string too long
            {
                goto Failed;
            }
            else if (parameterStartIndex < 0) // No parameters.
            {
                parameterStartIndex = 0;
            }


            ReadOnlySpan<char> mediaPartSpan = parameterStartIndex < 1 ? span : span.Slice(0, parameterStartIndex);
            int mediaTypeSeparatorIndex = mediaPartSpan.IndexOf('/');

            if (mediaTypeSeparatorIndex < 1)
            {
                goto Failed;
            }

            int topLevelMediaTypeLength = mediaPartSpan.Slice(0, mediaTypeSeparatorIndex).GetTrimmedLength();

            if (topLevelMediaTypeLength is 0 or > sbyte.MaxValue)
            {
                goto Failed;
            }

            int subTypeStart = mediaTypeSeparatorIndex + 1;
            subTypeStart += mediaPartSpan.Slice(subTypeStart).GetTrimmedStart();

            if (subTypeStart == mediaPartSpan.Length || subTypeStart > byte.MaxValue)
            {
                goto Failed;
            }

            int subTypeLength = mediaPartSpan.Slice(subTypeStart).GetTrimmedLength();

            int idx = topLevelMediaTypeLength << TOP_LEVEL_MEDIA_TYPE_LENGTH_SHIFT;
            idx |= subTypeStart << SUB_TYPE_START_SHIFT;
            idx |= subTypeLength << SUB_TYPE_LENGTH_SHIFT;
            idx |= parameterStartIndex;

            mimeType = new MimeType(
                in value,
                idx);

            return true;

/////////////////////////////////////////////////////////////
Failed:
            mimeType = default;
            return false;
        }

        /// <summary>
        /// Creates an appropriate <see cref="MimeType"/> instance for a given
        /// file type extension.
        /// </summary>
        /// <param name="fileTypeExtension">The file type extension to search for.</param>
        /// <returns>An appropriate <see cref="MimeType"/> instance for <paramref name="fileTypeExtension"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileTypeExtension"/> is <c>null</c>.</exception>
        public static MimeType FromFileTypeExtension(string fileTypeExtension)
        {
            if (fileTypeExtension is null)
            {
                throw new ArgumentNullException(nameof(fileTypeExtension));
            }
            else
            {
                ReadOnlyMemory<char> memory = MimeCache.GetMimeType(fileTypeExtension).AsMemory();
                _ = TryParse(ref memory, out MimeType inetMediaType);
                return inetMediaType;
            }
        }

        #endregion

    }
}
