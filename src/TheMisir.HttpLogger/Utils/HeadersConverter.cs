using System.Net.Http.Headers;

namespace TheMisir.HttpLogger.Utils;

public static class HeadersConverter
{
    public static IReadOnlyDictionary<string, string?> Convert(HttpHeaders headers)
    {
        return new Dictionary<string, string?>(from kvp in headers
            let value = kvp.Value.ToArray()
            select new KeyValuePair<string, string?>(kvp.Key, value switch
            {
                [] => null,
                [var single] => single,
                _ => string.Join(';', value)
            }));
    }

    public static IReadOnlyDictionary<string, string?> Convert(IHeaderDictionary headers)
    {
        return new Dictionary<string, string?>(from kvp in headers
            select new KeyValuePair<string, string?>(kvp.Key, kvp.Value switch
            {
                [] => null,
                var value => value.ToString()
            }));
    }
}
