using System;
using Unity.Collections;

namespace LostPolygon.Unity.Utility {
    public struct NativeArrayWithLength<T> : IDisposable where T : struct {
        public NativeArray<T> Array { get; }
        public NativeArray<int> Length { get; }

        public NativeArrayWithLength(NativeArray<T> array, int length, Allocator lengthAllocator) {
            Array = array;
            NativeArray<int> lengthArray = new(1, lengthAllocator, NativeArrayOptions.UninitializedMemory);
            lengthArray[0] = length;
            Length = lengthArray;
        }

        public NativeArray<T> GetSubArray() {
            return Array.GetSubArray(0, Length[0]);
        }

        public void Dispose() {
            Array.Dispose();
            Length.Dispose();
        }
    }
}
