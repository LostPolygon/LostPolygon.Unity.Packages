using System;
using log4net.Core;
using log4net.Repository;
using ILogger = log4net.Core.ILogger;

namespace LostPolygon.Log4netExtensions {
    /// <summary>
    /// Takes an existing logger and adds a [tag] to the beginning of each log message.
    /// </summary>
    public class TaggingLoggerDecorator : ILogger {
        public ILogger Delegate { get; }

        public string Tag { get; }

        public TaggingLoggerDecorator(ILogger delegateLogger, string prefix) {
            Delegate = delegateLogger ?? throw new ArgumentNullException(nameof(delegateLogger));
            Tag = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }

#if UNITY_EDITOR
        [UnityEngine.HideInCallstack]
#endif
        public void Log(Type callerStackBoundaryDeclaringType, Level level, object message, Exception exception) {
            if (message != null) {
                message = "[" + Tag + "] " + message;
            }

            Delegate.Log(callerStackBoundaryDeclaringType, level, message, exception);
        }

#if UNITY_EDITOR
        [UnityEngine.HideInCallstack]
#endif
        public void Log(LoggingEvent logEvent) {
            LoggingEventData loggingEventData = logEvent.GetLoggingEventData();

            if (loggingEventData.Message != null) {
                loggingEventData.Message = "[" + Tag + "] " + loggingEventData.Message;
            }

            loggingEventData.Properties["CustomTag"] = Tag;

            LoggingEvent loggingEvent = new LoggingEvent(loggingEventData);
            Delegate.Log(loggingEvent);
        }

        public bool IsEnabledFor(Level level) {
            return Delegate.IsEnabledFor(level);
        }

        public string Name => Delegate.Name;

        public ILoggerRepository Repository => Delegate.Repository;
    }
}
