using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace LostPolygon.Unity.Utility {
    public static unsafe class NativeCollectionsExtensions {
        public static Span<T> AsSpan<T>(this NativeArray<T> nativeArray) where T : struct {
            return new Span<T>(nativeArray.GetUnsafePtr(), nativeArray.Length);
        }

        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this NativeArray<T> nativeArray) where T : struct {
            return new ReadOnlySpan<T>(nativeArray.GetUnsafeReadOnlyPtr(), nativeArray.Length);
        }

        public static Span<T> AsSpan<T>(this NativeSlice<T> nativeSlice) where T : struct {
            return new Span<T>(nativeSlice.GetUnsafePtr(), nativeSlice.Length);
        }

        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this NativeSlice<T> nativeSlice) where T : struct {
            return new ReadOnlySpan<T>(nativeSlice.GetUnsafeReadOnlyPtr(), nativeSlice.Length);
        }
    }
}
