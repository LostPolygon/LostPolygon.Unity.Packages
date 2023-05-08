using System;
using System.Collections.Concurrent;
using System.IO;
using log4net.Core;
using log4net.Layout;
using log4net.Util;
using UnityEngine;

namespace LostPolygon.Log4netExtensions {
    public partial class NiceHtmlLayout : HtmlLayout {
        private readonly ConcurrentDictionary<string, string> _loggerNameToLoggerCellStyle = new();

        protected override void ProcessPatternLayout(PatternLayout patternLayout) {
            patternLayout.AddConverter("counter", typeof(CounterPatternConverter));
        }

        protected override bool IsFilteredPatternConverter(PatternConverter patternConverter) {
            switch (GetPatternConverterName(patternConverter)) {
                case "Logger":
                case "Message":
                    return true;
                default:
                    return false;
            }
        }

        protected override string GetLogItemCellClass(PatternConverter patternConverter, LoggingEvent loggingEvent) {
            return GetPatternConverterName(patternConverter) switch {
                "Time" => "text-monospace small",
                "#" => "text-monospace small text-center",
                "Logger" => "item-logger",
                "Message" => "preformatted",
                _ => ""
            };
        }

        protected override string GetLogItemHeaderCellClass(PatternConverter patternConverter) {
            return GetPatternConverterName(patternConverter) switch {
                "#" => "fit text-center",
                "Message" => "",
                _ => "fit"
            };
        }

        protected override string GetLogItemCellStyle(PatternConverter patternConverter, LoggingEvent loggingEvent) {
            switch (GetPatternConverterName(patternConverter)) {
                case "Logger":
                    string loggerName = loggingEvent.LoggerName;
                    if (!_loggerNameToLoggerCellStyle.TryGetValue(loggerName, out string style)) {
                        Color32 color = CalculateLoggerCellColor(loggingEvent);
                        style = $"color: #{color.r:x2}{color.g:x2}{color.b:x2};";

                        _loggerNameToLoggerCellStyle.TryAdd(loggerName, style);
                    }

                    return style;
                default:
                    return base.GetLogItemCellStyle(patternConverter, loggingEvent);
            }
        }

        protected override string CreatePatternConverterName(PatternConverter patternConverter) {
            string typeName = patternConverter.GetType().Name;
            return typeName switch {
                "UtcDatePatternConverter" => "Time",
                "CounterPatternConverter" => "#",
                _ => base.CreatePatternConverterName(patternConverter)
            };
        }

        protected override void WriteCell(LoggingEvent loggingEvent, PatternConverter patternConverter, TextWriter writer, TextWriter htmlWriter) {
            switch (GetPatternConverterName(patternConverter)) {
                case "Message":
                    base.WriteCell(loggingEvent, patternConverter, writer, htmlWriter);

                    // Write exception in the same cell, at the end of the cell
                    string exceptionString = loggingEvent.GetExceptionString();
                    if (!String.IsNullOrWhiteSpace(exceptionString)) {
                        writer.Write(@"<div class=""text-monospace small"">");
                        htmlWriter.Write(exceptionString);
                        writer.Write(@"</div>");
                    } else {
                        string stackTrace = StackTraceUtility.GetStackTraceString(loggingEvent);
                        if (!String.IsNullOrWhiteSpace(stackTrace)) {
                            writer.Write(@"<div class=""log-message-stacktrace text-monospace small"">");
                            htmlWriter.Write(stackTrace);
                            writer.Write(@"</div>");
                        }
                    }

                    break;
                default:
                    base.WriteCell(loggingEvent, patternConverter, writer, htmlWriter);
                    break;
            }
        }

        private static Color32 CalculateLoggerCellColor(LoggingEvent loggingEvent) {
            // Use name hashcode as hue
            double hue =
                StringFastHash(loggingEvent.LoggerName) /
                (double) UInt64.MaxValue;

            return Color.HSVToRGB((float) hue, 1f, 0.7f);
        }

        // Custom hash function to make sure the result is consistent
        private static ulong StringFastHash(string stringToHash) {
            ulong hashedValue = 3074457345618258791ul;
            for (int i = 0; i < stringToHash.Length; i++) {
                hashedValue += stringToHash[i];
                hashedValue *= 3074457345618258799ul;
            }

            return hashedValue;
        }
    }
}
