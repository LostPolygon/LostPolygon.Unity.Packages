using System.Text;
using Cysharp.Threading.Tasks;
using OneOf;
using UnityEngine.Scripting;

namespace LostPolygon.Unity.HttpClient {
    public class LoggingInterceptor : IInterceptor {
        [Preserve]
        public LoggingInterceptor() {
        }

        public async UniTask<OneOf<HttpResponse, IOErrorContext>> Intercept(IInterceptor.IChain chain) {
            HttpRequest request = await chain.Request();

#if LOG_REQUESTS
            string body = request.RequestBody != null ? Encoding.UTF8.GetString(request.RequestBody) : "";
            chain.Log.Debug($"{request.HttpVerb} request to {request.Url}:\n{body}");
#endif

            OneOf<HttpResponse, IOErrorContext> response = await chain.Proceed(request);

#if LOG_REQUESTS
            response.Switch(
                success => chain.Log.Debug($"Successful {request.HttpVerb} request to {request.Url}, response:\n{success.GetResponseAsUtf8()}"),
                error => HandleError(chain, request, error)
            );
#endif
            return response;
        }

        protected virtual void HandleError(IInterceptor.IChain chain, in HttpRequest request, in IOErrorContext errorContext) {
        }
    }
}
