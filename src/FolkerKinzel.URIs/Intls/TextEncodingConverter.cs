using System.Text;

namespace FolkerKinzel.URIs.Intls
{
    internal static class TextEncodingConverter
    {
        internal static Encoding GetEncoding(string? s)
        {
            if (s is null)
            {
                return Encoding.UTF8;
            }

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
