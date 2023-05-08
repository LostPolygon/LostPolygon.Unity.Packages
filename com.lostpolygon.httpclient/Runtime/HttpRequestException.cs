using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace LostPolygon.Unity.HttpClient {
    public class HttpRequestException : Exception {
        public UnityWebRequest.Result Result { get; }

        public string Error { get; }
        public string Text { get; }
        public long ResponseCode { get; }
        public IReadOnlyDictionary<string, string> ResponseHeaders { get; }

        private readonly UnityWebRequestException _unityWebRequestException;

        public HttpRequestException(UnityWebRequestException unityWebRequestException) {
            Result = unityWebRequestException.Result;
            Error = unityWebRequestException.Error;
            Text = unityWebRequestException.Text;
            ResponseCode = unityWebRequestException.ResponseCode;
            ResponseHeaders = unityWebRequestException.ResponseHeaders;

            _unityWebRequestException = unityWebRequestException;
        }

        public HttpRequestException(
            UnityWebRequest.Result result,
            string error,
            string text,
            long responseCode,
            IReadOnlyDictionary<string, string> responseHeaders,
            Exception innerException = null
        ) : base("", innerException) {
            Result = result;
            Error = error;
            Text = text;
            ResponseCode = responseCode;
            ResponseHeaders = responseHeaders;
        }

        public override string Message {
            get {
                if (_unityWebRequestException != null)
                    return _unityWebRequestException.Message;

                if (Text != null) {
                    return Error + Environment.NewLine + Text;
                } else {
                    return Error;
                }
            }
        }
    }
}
