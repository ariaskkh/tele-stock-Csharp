namespace Common.Extensions
{
    public static class StringExtension
    {
        public static string GetUrlWithQuery(this string url, Dictionary<string, string> parameters)
        {
            var queryString = string.Join("&", parameters.Select(KeyValue => $"{Uri.EscapeDataString(KeyValue.Key)}={Uri.EscapeDataString(KeyValue.Value)}"));
            return $"{url}?{queryString}";
        }
    }
}
