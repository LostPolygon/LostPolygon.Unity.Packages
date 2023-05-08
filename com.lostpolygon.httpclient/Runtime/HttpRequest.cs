#nullable enable

using System.Collections.Generic;

namespace LostPolygon.Unity.HttpClient {
    public readonly struct HttpRequest {
        public readonly string Url;
        public readonly string HttpVerb;
        public readonly byte[]? RequestBody;
        public readonly IDictionary<string, string> Headers;

        public HttpRequest(
            string url,
            string httpVerb,
            byte[]? requestBody,
            IDictionary<string, string>? headers
        ) {
            Url = url;
            HttpVerb = httpVerb;
            RequestBody = requestBody;
            Headers = headers ?? new Dictionary<string, string>();
        }
    }
}
