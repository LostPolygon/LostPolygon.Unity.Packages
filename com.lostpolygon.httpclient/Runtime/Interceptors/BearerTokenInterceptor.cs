using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneOf;
using UnityEngine.Scripting;

namespace LostPolygon.Unity.HttpClient {
    public class BearerTokenInterceptor : IInterceptor {
        public string BearerToken { get; set; }

        [Preserve]
        public BearerTokenInterceptor(string bearerToken) {
            BearerToken = bearerToken;
        }

        public async UniTask<OneOf<HttpResponse, IOErrorContext>> Intercept(IInterceptor.IChain chain) {
            HttpRequest request = await chain.Request();

            request = new HttpRequest(
                request.Url,
                request.HttpVerb,
                request.RequestBody,
                new Dictionary<string, string>(request.Headers) {
                    ["Authorization"] = "Bearer " + BearerToken
                }
            );

            return await chain.Proceed(request);
        }
    }
}
