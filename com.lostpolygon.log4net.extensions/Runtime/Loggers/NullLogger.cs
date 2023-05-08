using System;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace LostPolygon.Log4netExtensions {
    public class NullLogger : ILogger {
        private static readonly ILoggerRepository NullRepository = new Hierarchy();

        public void Log(Type callerStackBoundaryDeclaringType, Level level, object message, Exception exception) {
        }

        public void Log(LoggingEvent logEvent) {
        }

        public bool IsEnabledFor(Level level) {
            return false;
        }

        public string Name => nameof(NullLogger);

        public ILoggerRepository Repository => NullRepository;
    }
}
