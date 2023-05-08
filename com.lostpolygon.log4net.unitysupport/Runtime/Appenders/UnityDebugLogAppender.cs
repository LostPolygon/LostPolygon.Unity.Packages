using System;
using log4net.Appender;
using log4net.Core;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LostPolygon.Unity.Log4net {
    public class UnityDebugLogAppender : AppenderSkeleton {
        [HideInCallstack]
        protected override void Append(LoggingEvent loggingEvent) {
            if (loggingEvent.LoggerName == "Unity")
                return;

            if (String.IsNullOrEmpty(loggingEvent.RenderedMessage) &&
                loggingEvent.Level >= Level.Error &&
                loggingEvent.ExceptionObject != null
               ) {
                Debug.LogException(loggingEvent.ExceptionObject);
                return;
            }

            string message = RenderLoggingEvent(loggingEvent);
            LogMessage(loggingEvent, message);
        }

        [HideInCallstack]
        protected virtual void LogMessage(LoggingEvent loggingEvent, string message) {
            Debug.LogFormat(
                loggingEvent.Level switch {
                    Level lvl when lvl >= Level.Error => LogType.Error,
                    Level lvl when lvl >= Level.Warn => LogType.Warning,
                    _ => LogType.Log
                },
                LogOption.None,
                null,
                "{0}",
                message
            );
        }
    }
}
