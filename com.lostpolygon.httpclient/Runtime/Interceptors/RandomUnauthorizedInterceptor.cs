using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using OneOf;
using UnityEngine;
using UnityEngine.Networking;

namespace LostPolygon.Unity.HttpClient {
    /// Causes authorized successful responses to return 401 Unauthorized instead. For testing only!
    public class RandomUnauthorizedInterceptor : IInterceptor {
        public async UniTask<OneOf<HttpResponse, IOErrorContext>> Intercept(IInterceptor.IChain chain) {
            HttpRequest request = await chain.Request();
            OneOf<HttpResponse, IOErrorContext> result = await chain.Proceed(request);

            if (request.Headers.ContainsKey("Authorization") && result.IsT0 && Random.value > 0.8f) {
                chain.Log.Warn("Faking unauthorized response!!!");
                result = new IOErrorContext(
                    new HttpRequestException(
                        UnityWebRequest.Result.ProtocolError, "Fake Unauthorized response!", "", (long) HttpStatusCode.Unauthorized, new Dictionary<string, string>()
                    ),
                    null,
                    null
                );
            }

            return result;
        }
    }
}
