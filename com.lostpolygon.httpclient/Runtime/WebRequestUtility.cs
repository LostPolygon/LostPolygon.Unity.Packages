using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneOf;
using OneOf.Types;
using UnityEngine.Networking;

namespace LostPolygon.Unity.HttpClient {
    public static class WebRequestUtility {
        public static UniTask<OneOf<Success<UnityWebRequest>, IOError<(UnityWebRequest request, UnityWebRequestException exception)>>> SendRequest(
            string url,
            string httpVerb,
            byte[] body = null,
            IReadOnlyDictionary<string, string> headers = null,
            Action<UnityWebRequest> modifyWebRequestAction = null
        ) {
            UnityWebRequest webRequest = new UnityWebRequest(url, httpVerb);
            if (headers != null) {
                foreach (KeyValuePair<string, string> header in headers) {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            modifyWebRequestAction?.Invoke(webRequest);

            if (body?.Length != 0) {
                webRequest.uploadHandler = new UploadHandlerRaw(body);
            }

            webRequest.downloadHandler = new DownloadHandlerBuffer();

            return SendRequest(webRequest);
        }

        public static async UniTask<OneOf<Success<UnityWebRequest>, IOError<(UnityWebRequest request, UnityWebRequestException exception)>>>
            SendRequest(UnityWebRequest webRequest) {
            try {
                await webRequest.SendWebRequest();
                return new Success<UnityWebRequest>(webRequest);
            } catch (UnityWebRequestException e) {
                return new IOError<(UnityWebRequest, UnityWebRequestException)>((webRequest, e));
            }
        }
    }
}
