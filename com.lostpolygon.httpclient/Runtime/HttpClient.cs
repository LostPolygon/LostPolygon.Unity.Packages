using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using log4net;
using OneOf;

namespace LostPolygon.Unity.HttpClient {
    public class HttpClient : IHttpClient {
        private readonly IReadOnlyList<IInterceptor> _interceptors;
        private readonly ILog _log;

        public IReadOnlyList<IInterceptor> Interceptors => _interceptors;

        public HttpClient(List<IInterceptor> interceptors, ILog log) {
            _interceptors = interceptors;
            _log = log;
        }

        public async UniTask<OneOf<HttpResponse, IOErrorContext>> ExecuteRequest(HttpRequest request) {
            Chain chain = new Chain(_log, _interceptors, request);
            return await chain.Execute();
        }
    }
}
