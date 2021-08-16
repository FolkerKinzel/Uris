using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes;
using FolkerKinzel.Uris.Extensions;
using FolkerKinzel.Uris.Properties;


#if NET461 || NETSTANDARD2_0
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrlInfo : IEquatable<DataUrlInfo>, ICloneable
    {
        #region Parser

        /// <summary>
        /// Parses a <see cref="string"/> as <see cref="DataUrlInfo"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <returns>The <see cref="DataUrlInfo"/> instance, which <paramref name="value"/> represents.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="DataUrlInfo"/>.</exception>
        public static DataUrlInfo Parse(string value)
            => value is null
                ? throw new ArgumentNullException(nameof(value))
                : TryParse(value, out DataUrlInfo dataUrl)
                    ? dataUrl
                    : throw new ArgumentException(string.Format(Res.InvalidDataUrl, nameof(value)), nameof(value));


        /// <summary>
        /// Tries to parse a <see cref="string"/> as <see cref="DataUrlInfo"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <param name="dataUrl">If the method returns <c>true</c> the parameter contains a <see cref="DataUrlInfo"/> structure that provides the contents
        /// of value. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="DataUrlInfo"/>, <c>false</c> otherwise.</returns>
        public static bool TryParse(string? value, out DataUrlInfo dataUrl)
        {
            ReadOnlyMemory<char> memory = value.AsMemory();
            return TryParse(in memory, out dataUrl);
        }

        /// <summary>
        /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="DataUrlInfo"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
        /// <param name="dataUrl">If the method returns <c>true</c> the parameter contains a <see cref="DataUrlInfo"/> structure that provides the contents
        /// of value. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="DataUrlInfo"/>, <c>false</c> otherwise.</returns>
        public static bool TryParse(in ReadOnlyMemory<char> value, out DataUrlInfo dataUrl)
        {
            ReadOnlySpan<char> span = value.Span;

            if (!span.IsDataUrl())
            {
                goto Failed;
            }

            int mimeTypeEndIndex = -1;
            int startOfData = -1;

            for (int i = DataUrl.Protocol.Length; i < span.Length; i++)
            {
                char c = span[i];

                if (c == ',')
                {
                    startOfData = mimeTypeEndIndex = i;
                    break;
                }
            }

            if (mimeTypeEndIndex == -1) // missing ','
            {
                goto Failed;
            }

            // dies ändert ggf. auch mimeTypeEndIndex
            ReadOnlySpan<char> mimePart = span.Slice(DataUrl.Protocol.Length, mimeTypeEndIndex - DataUrl.Protocol.Length);
            ContentEncoding dataEncoding = ContentEncoding.Url;

            if (HasBase64Encoding(mimePart))
            {
                mimePart = mimePart.Slice(0, mimePart.Length - DataUrl.Base64.Length);
                mimeTypeEndIndex -= DataUrl.Base64.Length;
                dataEncoding = ContentEncoding.Base64;
            }

            MimeType mediaType;

            if (mimePart.IsEmpty)
            {
                mediaType = DataUrlInfo.DefaultMediaType();
            }
            else
            {
                ReadOnlyMemory<char> memory = span[DataUrl.Protocol.Length] == ';'
                                                ? new StringBuilder(DEFAULT_MEDIA_TYPE.Length + mimePart.Length)
                                                    .Append(DEFAULT_MEDIA_TYPE)
                                                    .Append(mimePart).ToString()
                                                    .AsMemory()
                                                : value.Slice(DataUrl.Protocol.Length, mimeTypeEndIndex - DataUrl.Protocol.Length);

                if (!MimeType.TryParse(ref memory, out mediaType))
                {
                    goto Failed;
                }
            }
            ReadOnlyMemory<char> embeddedData = value.Slice(startOfData + 1);
            dataUrl = new DataUrlInfo(in mediaType, dataEncoding, in embeddedData);

            return true;

Failed:
            dataUrl = default;
            return false;


            //////////////////////////////////////////////////////////////

            static bool HasBase64Encoding(ReadOnlySpan<char> val)
            {
                //Suche ";base64"
                if (val.Length < DataUrl.Base64.Length)
                {
                    return false;
                }

                ReadOnlySpan<char> hayStack = val.Slice(val.Length - DataUrl.Base64.Length);

                for (int i = 0; i < hayStack.Length; i++)
                {
                    char c = char.ToLowerInvariant(hayStack[i]);

                    if (c != DataUrl.Base64[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        internal static MimeType DefaultMediaType()
        {
            ReadOnlyMemory<char> memory = DEFAULT_MEDIA_TYPE.AsMemory();
            _ = MimeType.TryParse(ref memory, out MimeType mediaType);
            return mediaType;
        }

        #endregion

    }
}
