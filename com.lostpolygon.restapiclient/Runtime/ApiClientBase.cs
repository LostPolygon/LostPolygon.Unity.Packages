using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Cysharp.Threading.Tasks;
using LostPolygon.Unity.HttpClient;
using log4net;
using OneOf;
using UnityEngine.Networking;

namespace LostPolygon.Unity.RestApiClient {
    public abstract class ApiClientBase<TError> {
        private readonly IHttpClient _httpClient;
        private readonly string _apiEndpoint;

        protected ILog Log { get; }

        public IHttpClient HttpClient => _httpClient;

        public string ApiEndpoint => _apiEndpoint;

        public ApiClientBase(IHttpClient httpClient, string apiEndpoint, ILog log = null) {
            Log = log;
            _httpClient = httpClient;
            _apiEndpoint = apiEndpoint;
        }

        public async UniTask<OneOf<string, TError>> GetString(
            string url,
            [CallerMemberName] string operationName = null
        ) {
            HttpRequest request = new HttpRequest(
                _apiEndpoint + url,
                UnityWebRequest.kHttpVerbGET,
                null,
                null
            );

            OneOf<HttpResponse, IOErrorContext> response = await _httpClient.ExecuteRequest(request);
            return HandleResponse(request, response, operationName);
        }

        public async UniTask<OneOf<T, TError>> GetJson<T>(
            string url,
            [CallerMemberName] string operationName = null
        ) {
            OneOf<string, TError> response = await GetString(url, operationName);
            return HandleJsonResponse<T>(response, operationName);
        }

        public async UniTask<OneOf<string, TError>> PostJson(
            string url,
            string jsonBody,
            [CallerMemberName] string operationName = null
        ) {
            HttpRequest request = new HttpRequest(
                _apiEndpoint + url,
                UnityWebRequest.kHttpVerbPOST,
                jsonBody != null ? Encoding.UTF8.GetBytes(jsonBody) : null,
                new Dictionary<string, string> {
                    ["Content-Type"] = "application/json"
                }
            );

            OneOf<HttpResponse, IOErrorContext> response = await _httpClient.ExecuteRequest(request);
            return HandleResponse(request, response, operationName);
        }

        public async UniTask<OneOf<T, TError>> PostJson<T>(
            string url,
            string jsonBody,
            [CallerMemberName] string operationName = null
        ) {
            OneOf<string, TError> response = await PostJson(url, jsonBody, operationName);
            return HandleJsonResponse<T>(response, operationName);
        }

        protected abstract TError CreateError(IOErrorContext errorContext);

        protected abstract OneOf<TResult, TError> DeserializeJsonResponse<TResult>(string responseBody, string operationName);

        private OneOf<string, TError> HandleResponse(HttpRequest request, OneOf<HttpResponse, IOErrorContext> response, string operationName) {
            return response.Match<OneOf<string, TError>>(
                success => success.GetResponseAsUtf8(),
                error => CreateError(error)
            );
        }

        private OneOf<T, TError> HandleJsonResponse<T>(OneOf<string, TError> response, string operationName) {
            return response.Match(
                responseBody => DeserializeJsonResponse<T>(responseBody, operationName),
                error => error
            );
        }
    }
}
