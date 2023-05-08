#nullable enable

using Cysharp.Threading.Tasks;
using log4net;
using OneOf;

namespace LostPolygon.Unity.HttpClient {
    public interface IInterceptor {
        UniTask<OneOf<HttpResponse, IOErrorContext>> Intercept(IChain chain);

        public interface IChain {
            ILog Log { get; }
            UniTask<HttpRequest> Request();
            UniTask<OneOf<HttpResponse, IOErrorContext>> Proceed(HttpRequest request);
        }
    }
}
