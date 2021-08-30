using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes;
using FolkerKinzel.Strings.Polyfills;
using FolkerKinzel.Uris.Extensions;
using FolkerKinzel.Uris.Properties;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrlInfo
    {
        
        #region Parser

        /// <summary>
        /// Parses a <see cref="string"/> that represents a "data" URL as <see cref="DataUrlInfo"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <returns>A <see cref="DataUrlInfo"/> instance, which represents <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> could not be parsed as <see cref="DataUrlInfo"/>.</exception>
        public static DataUrlInfo Parse(string value)
            => value is null
                ? throw new ArgumentNullException(nameof(value))
                : TryParse(value, out DataUrlInfo dataUrl)
                    ? dataUrl
                    : throw new ArgumentException(string.Format(Res.InvalidDataUrl, nameof(value)), nameof(value));

        /// <summary>
        /// Parses a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that represents a "data" URL as <see cref="DataUrlInfo"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
        /// <returns>A <see cref="DataUrlInfo"/> instance, which represents <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> could not be parsed as <see cref="DataUrlInfo"/>.</exception>
        public static DataUrlInfo Parse(ReadOnlyMemory<char> value)
            => TryParse(value, out DataUrlInfo dataUrl)
                    ? dataUrl
                    : throw new ArgumentException(string.Format(Res.InvalidDataUrl, nameof(value)), nameof(value));


        /// <summary>
        /// Tries to parse a <see cref="string"/> that represents a "data" URL in order to make its content available as <see cref="DataUrlInfo"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <param name="info">If the method returns <c>true</c>, the parameter contains a <see cref="DataUrlInfo"/> structure 
        /// that provides the contents of the "data" URL. The argument is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="DataUrlInfo"/>, otherwise <c>false</c>.</returns>
        public static bool TryParse(string? value, out DataUrlInfo info)
        {
            ReadOnlyMemory<char> memory = value.AsMemory();
            return TryParse(in memory, out info);
        }

        /// <summary>
        /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that represents a "data" URL 
        /// as <see cref="DataUrlInfo"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
        /// <param name="info">If the method returns <c>true</c>, the parameter contains a <see cref="DataUrlInfo"/> 
        /// structure that provides the contents
        /// of the "data" URL. The argument is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="DataUrlInfo"/>, <c>false</c> otherwise.</returns>
        public static bool TryParse(in ReadOnlyMemory<char> value, out DataUrlInfo info)
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
            DataEncoding dataEncoding = DataEncoding.Url;

            if (HasBase64Encoding(mimePart))
            {
                mimePart = mimePart.Slice(0, mimePart.Length - DataUrl.Base64.Length);
                mimeTypeEndIndex -= DataUrl.Base64.Length;
                dataEncoding = DataEncoding.Base64;
            }

            MimeType mediaType;

            if (mimePart.IsEmpty)
            {
                mediaType = DataUrl.DefaultMediaType();
            }
            else
            {
                ReadOnlyMemory<char> memory = span[DataUrl.Protocol.Length] == ';'
                                                ? new StringBuilder(DataUrl.DEFAULT_MEDIA_TYPE.Length + mimePart.Length)
                                                    .Append(DataUrl.DEFAULT_MEDIA_TYPE)
                                                    .Append(mimePart).ToString()
                                                    .AsMemory()
                                                : value.Slice(DataUrl.Protocol.Length, mimeTypeEndIndex - DataUrl.Protocol.Length);

                if (!MimeType.TryParse(ref memory, out mediaType))
                {
                    goto Failed;
                }
            }
            ReadOnlyMemory<char> embeddedData = value.Slice(startOfData + 1);
            info = new DataUrlInfo(in mediaType, dataEncoding, in embeddedData);

            return true;

Failed:
            info = default;
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

        

        #endregion


    }
}
