using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using OneOf;

namespace LostPolygon.Unity.RestApiClient {
    public static class ApiClientExtensions {
        public static async UniTask<OneOf<TModel, TError>> GetJson<TDto, TModel, TError>(
            this ApiClientBase<TError> apiClient,
            string url,
            Func<TDto, TModel> dtoToModelMapFunc,
            [CallerMemberName] string operationName = null
        ) {
            OneOf<TDto, TError> dtoResult = await apiClient.GetJson<TDto>(url, operationName);

            OneOf<TModel, TError> result =
                dtoResult.Match(
                    dto => (OneOf<TModel, TError>) dtoToModelMapFunc(dto),
                    error => error
                );

            return result;
        }

        public static async UniTask<OneOf<TModel, TError>> PostJson<TDto, TModel, TError>(
            this ApiClientBase<TError> apiClient,
            string url,
            string jsonBody,
            Func<TDto, TModel> dtoToModelMapFunc,
            [CallerMemberName] string operationName = null
        ) {
            OneOf<TDto, TError> dtoResult = await apiClient.PostJson<TDto>(url, jsonBody, operationName);
            OneOf<TModel, TError> result =
                dtoResult.Match(
                    dto => (OneOf<TModel, TError>) dtoToModelMapFunc(dto),
                    error => error
                );

            return result;
        }
    }
}
