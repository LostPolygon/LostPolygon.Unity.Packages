using System;
using System.IO;
using log4net.Core;
using log4net.Layout.Pattern;

namespace LostPolygon.Log4netExtensions {
#if UNITY_2019_1_OR_NEWER
    [UnityEngine.Scripting.Preserve]
#endif
    public class ExceptionPaddingPatternConverter : PatternLayoutConverter {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent) {
            if (loggingEvent.ExceptionObject != null &&
                !String.IsNullOrEmpty(loggingEvent.RenderedMessage) &&
                !String.IsNullOrEmpty(Option)
               ) {
                writer.Write(Option);
            }
        }
    }
}
