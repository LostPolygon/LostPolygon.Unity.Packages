using System;
using System.Diagnostics;

namespace LostPolygon.Unity.Utility {
    public readonly struct ValueStopwatch {
        private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double) Stopwatch.Frequency;

        private readonly long _startTimestamp;

        private ValueStopwatch(long startTimestamp) => _startTimestamp = startTimestamp;

        public static ValueStopwatch StartNew() => new(GetTimestamp());

        public static long GetTimestamp() => Stopwatch.GetTimestamp();

        public static TimeSpan GetElapsedTime(long startTimestamp, long endTimestamp) {
            long timestampDelta = endTimestamp - startTimestamp;
            long ticks = (long) (TimestampToTicks * timestampDelta);
            return new TimeSpan(ticks);
        }

        public TimeSpan GetElapsedTime() => GetElapsedTime(_startTimestamp, GetTimestamp());
    }
}
