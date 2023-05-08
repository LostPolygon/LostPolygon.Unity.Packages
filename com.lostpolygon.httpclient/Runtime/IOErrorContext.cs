#nullable enable

namespace LostPolygon.Unity.HttpClient {
    public readonly struct IOErrorContext {
        public readonly HttpRequestException Exception;
        public readonly HttpResponse? Response;
        public readonly string? OperationName;

        public IOErrorContext(HttpRequestException exception, HttpResponse? response, string? operationName) {
            Exception = exception;
            Response = response;
            OperationName = operationName;
        }
    }
}
