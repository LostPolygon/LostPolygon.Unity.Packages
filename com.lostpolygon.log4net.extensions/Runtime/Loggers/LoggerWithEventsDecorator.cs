using System;
using log4net.Core;
using log4net.Repository;

namespace LostPolygon.Log4netExtensions {
    public class LoggerWithEventsDecorator : ILogger {
        public event Action<string> Message;

        public bool LogToDelegate { get; }
        public ILogger DelegateLogger { get; }

        public LoggerWithEventsDecorator(ILogger delegateLogger, bool logToDelegate = true) {
            LogToDelegate = logToDelegate;
            DelegateLogger = delegateLogger;
        }

        public void Log(Type callerStackBoundaryDeclaringType, Level level, object message, Exception exception) {
            if (LogToDelegate) {
                DelegateLogger.Log(callerStackBoundaryDeclaringType, level, message, exception);
            }

            Message?.Invoke(message?.ToString());
        }

        public void Log(LoggingEvent logEvent) {
            if (LogToDelegate) {
                DelegateLogger.Log(logEvent);
            }

            Message?.Invoke(logEvent?.RenderedMessage);
        }

        public bool IsEnabledFor(Level level) {
            return DelegateLogger.IsEnabledFor(level);
        }

        public string Name => DelegateLogger.Name;

        public ILoggerRepository Repository => DelegateLogger.Repository;
    }
}
