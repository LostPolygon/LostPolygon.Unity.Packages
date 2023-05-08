#nullable enable

using System.Net;
using System.Text;

namespace LostPolygon.Unity.HttpClient {
    public readonly struct HttpResponse {
        public readonly HttpStatusCode StatusCode;
        public readonly byte[]? Response;

        public HttpResponse(HttpStatusCode statusCode, byte[]? response) {
            StatusCode = statusCode;
            Response = response;
        }

        public string? GetResponseAsUtf8() {
            if (Response == null)
                return null;

            return Encoding.UTF8.GetString(Response);
        }
    }
}
