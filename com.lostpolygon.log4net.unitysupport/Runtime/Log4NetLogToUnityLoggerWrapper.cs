using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Log4net {
    public class Log4NetLogToUnityLoggerWrapper : UnityEngine.ILogger {
        private readonly log4net.ILog _log;

        public Log4NetLogToUnityLoggerWrapper(log4net.ILog log4NetLog) {
            _log = log4NetLog ?? throw new ArgumentNullException(nameof(log4NetLog));
        }

        public bool IsLogTypeAllowed(LogType logType) {
            return logType switch {
                LogType.Error => _log.IsErrorEnabled,
                LogType.Assert => _log.IsErrorEnabled,
                LogType.Warning => _log.IsWarnEnabled,
                LogType.Log => _log.IsInfoEnabled,
                LogType.Exception => _log.IsErrorEnabled,
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args) {
            Log(logType, String.Format(format, args));
        }

        public void LogException(Exception exception, Object context) {
            Log(LogType.Exception, null, exception, context);
        }

        public void Log(LogType logType, object message) {
            Log(logType, null, message, null);
        }

        public void Log(LogType logType, object message, Object context) {
            Log(logType, null, message, context);
        }

        public void Log(LogType logType, string tag, object message) {
            Log(logType, tag, message, null);
        }

        protected virtual bool FilterLog(LogType logType, string tag, object message, Object context) {
            return true;
        }

        public void Log(LogType logType, string tag, object message, Object context) {
            if (!FilterLog(logType, tag, message, context))
                return;

            string format = IsEditorProvider.IsEditor ?
                "<i>[{0}]</i> {1}" :
                "[{0}] {1}";

            switch (logType) {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    if (message is Exception exception) {
                        if (String.IsNullOrEmpty(tag)) {
                            _log.Error("", exception);
                        } else {
                            _log.Error(String.Format(format, tag, ""), exception);
                        }
                    } else {
                        if (String.IsNullOrEmpty(tag)) {
                            _log.Error(message);
                        } else {
                            _log.ErrorFormat(format, tag, message);
                        }
                    }

                    break;
                case LogType.Warning:
                    if (String.IsNullOrEmpty(tag)) {
                        _log.Warn(message);
                    } else {
                        _log.WarnFormat(format, tag, message);
                    }

                    break;
                case LogType.Log:
                    if (String.IsNullOrEmpty(tag)) {
                        _log.Info(message);
                    } else {
                        _log.InfoFormat(format, tag, message);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }

        public void Log(object message) {
            Log(LogType.Log, null, message, null);
        }

        public void Log(string tag, object message) {
            Log(LogType.Log, tag, message, null);
        }

        public void Log(string tag, object message, Object context) {
            Log(LogType.Log, tag, message, context);
        }

        public void LogWarning(string tag, object message) {
            Log(LogType.Warning, tag, message, null);
        }

        public void LogWarning(string tag, object message, Object context) {
            Log(LogType.Warning, tag, message, context);
        }

        public void LogError(string tag, object message) {
            Log(LogType.Error, tag, message, null);
        }

        public void LogError(string tag, object message, Object context) {
            Log(LogType.Error, tag, message, context);
        }

        public void LogFormat(LogType logType, string format, params object[] args) {
            Log(logType, String.Format(format, args));
        }

        public void LogException(Exception exception) {
            LogException(exception, null);
        }

        public ILogHandler logHandler { get; set; }

        public bool logEnabled {
            get => true;
            set {
            }
        }

        public LogType filterLogType {
            get => LogType.Log;
            set {
            }
        }
    }
}
