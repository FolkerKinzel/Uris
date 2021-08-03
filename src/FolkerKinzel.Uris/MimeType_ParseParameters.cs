using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        #region ParseParameters

           private IEnumerable<MimeTypeParameter> ParseParameters()
        {
            int parameterStartIndex = _idx & 0xFF;

            if(parameterStartIndex < 1)
            {
                yield break;
            }

            do
            {
                if (TryParseParameter(ref parameterStartIndex, out MimeTypeParameter parameter))
                {
                    yield return parameter;
                }
            }
            while (parameterStartIndex != -1);
        }


        private bool TryParseParameter(ref int parameterStartIndex, out MimeTypeParameter parameter)
        {
            int nextParameterSeparatorIndex = GetNextParameterSeparatorIndex(_mimeTypeString.Span.Slice(parameterStartIndex));
            ReadOnlyMemory<char> currentParameterString;

            if (nextParameterSeparatorIndex < 0) // last parameter
            {
                currentParameterString = _mimeTypeString.Slice(parameterStartIndex);
                parameterStartIndex = -1;
            }
            else
            {
                currentParameterString = _mimeTypeString.Slice(parameterStartIndex, nextParameterSeparatorIndex);
                parameterStartIndex += nextParameterSeparatorIndex + 1;
            }

            return MimeTypeParameter.TryParse(ref currentParameterString, out parameter);
        }


        private static int GetNextParameterSeparatorIndex(ReadOnlySpan<char> value)
        {
            bool isInQuotes = false;

            for (int i = 0; i < value.Length; i++)
            {
                char current = value[i];

                if(current == '\\') // Mask char: Skip one!
                {
                    i++;
                    continue;
                }

                if (current == '"')
                {
                    isInQuotes = !isInQuotes;
                }

                if (isInQuotes)
                {
                    continue;
                }

                if (current == ';')
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

    }
}
