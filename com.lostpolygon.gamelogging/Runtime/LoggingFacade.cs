#nullable enable

using System.Collections.Generic;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Logger = log4net.Repository.Hierarchy.Logger;

namespace LostPolygon.Unity.GameLogging {
    public abstract class LoggingFacade {
        private readonly string _repositoryName;

        public ILog UnityLog { get; }

        public ILoggerRepository Repository { get; }

        protected LoggingFacade(string? repositoryName = null) {
            _repositoryName = repositoryName ?? "LogRepository";

            Repository =
                LoggerManager.RepositorySelector.ExistsRepository(_repositoryName) ?
                    LogManager.GetRepository(_repositoryName) :
                    LogManager.CreateRepository(_repositoryName);

            UnityLog = GetLog("Unity");
        }

        public ILog GetLog(string name) {
            return LogManager.GetLogger(_repositoryName, name);
        }

        public Logger GetLogger(string name) {
            return (Logger) GetLog(name).Logger;
        }

        protected virtual void ConfigureHierarchy(IEnumerable<IAppender> appenders) {
            Hierarchy hierarchy = (Hierarchy) Repository;
            foreach (IAppender appender in appenders) {
                hierarchy.Root.AddAppender(appender);
            }

            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }
    }
}
