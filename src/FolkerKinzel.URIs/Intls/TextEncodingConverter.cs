using System.Text;

namespace FolkerKinzel.Uris.Intls
{
    internal static class TextEncodingConverter
    {
        internal static Encoding GetEncoding(string? s)
        {
            if (s is null)
            {
                return Encoding.UTF8;
            }

#if NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            try
            {
                return Encoding.GetEncoding(s);
            }
            catch
            {
                return Encoding.UTF8;
            }
        }
    }
}
