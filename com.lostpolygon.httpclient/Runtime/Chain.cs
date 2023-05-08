using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using log4net;
using OneOf;
using UnityEngine.Networking;

namespace LostPolygon.Unity.HttpClient {
    internal class Chain : IInterceptor.IChain {
        private readonly IReadOnlyList<IInterceptor> _interceptors;

        private HttpRequest _currentRequest;
        private int _currentInterceptor;
        private OneOf<HttpResponse, IOErrorContext> _currentResponse;
        private OneOf<HttpResponse, IOErrorContext> _finalResponse;

        public Chain(ILog log, IReadOnlyList<IInterceptor> interceptors, HttpRequest request) {
            Log = log;
            _interceptors = interceptors;
            _currentRequest = request;
            _currentInterceptor = 0;
        }

        public ILog Log { get; }

        public async UniTask<OneOf<HttpResponse, IOErrorContext>> Execute() {
            return await Proceed(_currentRequest);
        }

        public UniTask<HttpRequest> Request() {
            return UniTask.FromResult(_currentRequest);
        }

        public async UniTask<OneOf<HttpResponse, IOErrorContext>> Proceed(HttpRequest request) {
            if (_currentInterceptor >= _interceptors.Count) {
                _currentInterceptor--;
                return await ExecuteRequest(request);
            }

            _currentRequest = request;
            IInterceptor interceptor = _interceptors[_currentInterceptor];
            _currentInterceptor++;

            OneOf<HttpResponse, IOErrorContext> result = await interceptor.Intercept(this);
            return result;
        }

        private static async UniTask<OneOf<HttpResponse, IOErrorContext>> ExecuteRequest(HttpRequest request) {
            var result = await WebRequestUtility.SendRequest(
                request.Url,
                request.HttpVerb,
                request.RequestBody,
                (IReadOnlyDictionary<string, string>) request.Headers
            );

            if (result.IsT1) {
                UnityWebRequestException unityWebRequestException = result.AsT1.Value.exception;
                try {
                    return new IOErrorContext(
                        new HttpRequestException(unityWebRequestException),
                        unityWebRequestException.Result != UnityWebRequest.Result.ConnectionError ?
                            new HttpResponse(
                                (HttpStatusCode) unityWebRequestException.ResponseCode,
                                unityWebRequestException.UnityWebRequest.downloadHandler.data
                            ) :
                            null,
                        null
                    );
                } finally {
                    unityWebRequestException.UnityWebRequest.Dispose();
                }
            }

            try {
                return new HttpResponse(
                    (HttpStatusCode) result.AsT0.Value.responseCode,
                    result.AsT0.Value.downloadHandler.data
                );
            } finally {
                result.AsT0.Value.Dispose();
            }
        }
    }
}
