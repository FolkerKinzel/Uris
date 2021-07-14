namespace FolkerKinzel.Uris.Intls
{
    internal static class DataUrl
    {
        private const string DEFAULT_MEDIA_TYPE = "text/plain";

        internal static InternetMediaType DefaultMediaType()
        {
            _ = InternetMediaType.TryParse(DEFAULT_MEDIA_TYPE, out InternetMediaType mediaType);
            return mediaType;
        }
    }
}
