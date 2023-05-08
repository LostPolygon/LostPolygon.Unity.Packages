using Cysharp.Threading.Tasks;
using OneOf;

namespace LostPolygon.Unity.HttpClient {
    public interface IHttpClient {
        UniTask<OneOf<HttpResponse, IOErrorContext>> ExecuteRequest(HttpRequest request);
    }
}
