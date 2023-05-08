using System;
using System.Text;
using log4net.Core;

namespace LostPolygon.Log4netExtensions {
    public static class StackTraceUtility {
        public const string LoggingEventStackTraceProperty = "stacktrace";

        public static string GetStackTraceString(LoggingEvent loggingEvent) {
            if (loggingEvent.Properties[LoggingEventStackTraceProperty] is not string stackTrace) {
                stackTrace = FormatStackTraceString(loggingEvent);
                loggingEvent.Properties[LoggingEventStackTraceProperty] = stackTrace;
            }

            return stackTrace;
        }

        public static string AddLinksToStackTrace(string stacktrace) {
            StringBuilder sb = new(stacktrace.Length + stacktrace.Length / 10);

            int position = 0;
            while (true) {
                const string fileRefPhraseStart = "(at ";
                const string fileRefPhraseEnd = ")";
                int fileRefPhraseStartIndex = stacktrace.IndexOf(fileRefPhraseStart, position, StringComparison.Ordinal);
                if (fileRefPhraseStartIndex == -1)
                    break;

                int fileRefPhraseEndIndex = stacktrace.IndexOf(fileRefPhraseEnd, fileRefPhraseStartIndex, StringComparison.Ordinal);
                if (fileRefPhraseEndIndex == -1)
                    break;

                int fileNameStartIndex = fileRefPhraseStartIndex + fileRefPhraseStart.Length;
                int fileLineColonIndex = stacktrace.IndexOf(":", fileNameStartIndex, StringComparison.Ordinal);
                if (fileLineColonIndex == -1)
                    break;

                int fileNameEndIndex = fileLineColonIndex;
                int fileLineStartIndex = fileLineColonIndex + 1;
                int fileLineEndIndex = fileRefPhraseEndIndex;

                sb.Append(stacktrace, position, fileRefPhraseStartIndex - position);
                sb.Append(fileRefPhraseStart);

                sb.Append(@"<a href=""");
                sb.Append(stacktrace, fileNameStartIndex, fileNameEndIndex - fileNameStartIndex);
                sb.Append(@""" line=""");
                sb.Append(stacktrace, fileLineStartIndex, fileLineEndIndex - fileLineStartIndex);
                sb.Append(@""">");
                sb.Append(stacktrace, fileNameStartIndex, fileRefPhraseEndIndex - fileNameStartIndex);
                sb.Append(@"</a>");
                sb.Append(fileRefPhraseEnd);

                position = fileRefPhraseEndIndex + fileRefPhraseEnd.Length;
            }

            int remainder = stacktrace.Length - position;
            if (remainder > 0) {
                sb.Append(stacktrace, position, remainder);
            }

            return sb.ToString();
        }

        private static string FormatStackTraceString(LoggingEvent loggingEvent) {
            return FormatStackTraceString(loggingEvent.LocationInformation.StackFrames);
        }

        private static string FormatStackTraceString( StackFrameItem[] stackFrames) {
            if (stackFrames == null || stackFrames.Length == 0)
                return "";

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < stackFrames.Length; i++) {
                StackFrameItem frame = stackFrames[i];
                stringBuilder.Append(frame.ClassName);
                stringBuilder.Append('.');
                AppendMethodInformation(stringBuilder, frame.Method);

                if (!String.IsNullOrEmpty(frame.FileName)) {
                    stringBuilder.Append(" (");
                    stringBuilder.Append(frame.FileName);
                    stringBuilder.Append(':');
                    stringBuilder.Append(frame.LineNumber);
                    stringBuilder.Append(')');
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        private static void AppendMethodInformation(StringBuilder stringBuilder, MethodItem method) {
            try {
                stringBuilder.Append(method.Name);
                stringBuilder.Append('(');

                string[] parameters = method.Parameters;
                int? upperBound = parameters?.GetUpperBound(0);
                if (upperBound > 0) {
                    for (int i = 0; i <= upperBound; ++i) {
                        stringBuilder.Append(parameters[i]);
                        if (i != upperBound) {
                            stringBuilder.Append(", ");
                        }
                    }
                }

                stringBuilder.Append(')');
            } catch (Exception ex) {
                stringBuilder.AppendLine("An exception occurred while retrieving method information. " + ex);
            }
        }
    }
}
