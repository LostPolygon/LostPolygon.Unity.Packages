using System.Globalization;
using System.IO;
using log4net.Core;
using log4net.Layout.Pattern;

namespace LostPolygon.Log4netExtensions {
    /// <summary>
    /// Writes in increasing integer counter. Useful for referencing a specific log item.
    /// </summary>
#if UNITY_2019_1_OR_NEWER
    [UnityEngine.Scripting.Preserve]
#endif
    public class CounterPatternConverter : PatternLayoutConverter {
        private int _counter;

        public void ResetCounter() {
            _counter = 0;
        }

        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent) {
            _counter++;
            writer.Write(_counter.ToString(CultureInfo.InvariantCulture));
        }
    }
}
