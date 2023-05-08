#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using log4net.Appender;
using log4net.Filter;
using LostPolygon.Unity.GameLogging;
using LostPolygon.Unity.Log4net;
using UnityEngine;

namespace LostPolygon.Unity.GameLogging {
    public class GameLoggingFacade : LoggingFacade, IDisposable {
        private bool _isConfigured;
        private string? _logFileName;
        private string? _logFileTitle;
        private IFilter[] _logFilters = Array.Empty<IFilter>();
        private UnityDebugLogHandler? _unityDebugLogHandler;
        private bool _unityLogsProxying;

        public GameLoggingFacade(string? repositoryName = null) : base(repositoryName) {
        }

        public GameLoggingFacade AddUnityLogsProxying(bool enabled = true) {
            _unityLogsProxying = enabled;
            return this;
        }

        public GameLoggingFacade AddFileLog(string logFileName, string logFileTitle) {
            _logFileName = logFileName;
            _logFileTitle = logFileTitle;
            return this;
        }

        public GameLoggingFacade SetLogFilters(IFilter[] logFilters) {
            _logFilters = logFilters;
            return this;
        }

        public GameLoggingFacade Configure() {
            if (_isConfigured)
                throw new InvalidOperationException("Logging already configured");

            _isConfigured = true;
            _unityDebugLogHandler = new UnityDebugLogHandler(UnityLog.Logger, _logFilters);
            if (_unityLogsProxying) {
                _unityDebugLogHandler.StartUnityLogsProxying();
            }

            List<IAppender> appenders = new List<IAppender> {
                _unityDebugLogHandler.Appender
            };

            if (_logFileName != null) {
                IReadOnlyList<IAppender> htmlLogAppenders =
                    NiceHtmlFileLogAppenderFactory.CreateAppenders(
                        GetLogFilePath(),
                        _logFileTitle,
                        _logFilters
                    );

                appenders.AddRange(htmlLogAppenders);
            }

            ConfigureHierarchy(appenders);
            return this;
        }

        public string GetLogFilePath() {
            if (_logFileName == null)
                throw new InvalidOperationException("File logging is not set up");

            string path = Path.Combine(Application.persistentDataPath, _logFileName);
            return Path.GetFullPath(path);
        }

        public void Dispose() {
            _unityDebugLogHandler?.Dispose();
            _unityDebugLogHandler = null;
        }
    }
}
