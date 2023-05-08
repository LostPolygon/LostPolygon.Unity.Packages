using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace LostPolygon.Unity.Utility {
    public static class SpanNativeCollectionsExtensions {
        public static NativeArray<T> AsNativeArrayUnsafe<T>(this ReadOnlySpan<T> span) where T : unmanaged {
            return AsNativeArrayUnsafe<T, T>(span);
        }

        public static unsafe NativeArray<TTo> AsNativeArrayUnsafe<TFrom, TTo>(this ReadOnlySpan<TFrom> span)
            where TFrom : unmanaged
            where TTo : unmanaged {
            fixed (TFrom* spanPtr = span) {
                return AsNativeArrayUnsafe<TTo>((IntPtr) spanPtr, span.Length);
            }
        }

        public static unsafe NativeArray<TTo> AsNativeArrayUnsafe<TTo>(this IntPtr ptr, int length)
            where TTo : unmanaged {
            return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<TTo>(
                (void*) ptr,
                length,
                Allocator.None
            );
        }
    }
}
