using System;
using System.Collections.Generic;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Util;
using LostPolygon.Log4netExtensions;
using UnityEngine;
using ILogger = log4net.Core.ILogger;
using StackTraceUtility = LostPolygon.Log4netExtensions.StackTraceUtility;

namespace LostPolygon.Unity.Log4net {
    public class UnityDebugLogHandler : IDisposable {
        public ILogger UnityLogger { get; }
        public bool CanUseRichText { get; }

        public UnityDebugLogAppender Appender { get; }

        public UnityDebugLogHandler(ILogger unityLogger, IEnumerable<IFilter> logFilters) {
            UnityLogger = unityLogger;
            CanUseRichText = Application.isEditor && !Application.isBatchMode;

            PatternLayout unityConsolePattern = new PatternLayout {
                ConversionPattern =
                    (CanUseRichText ? "<i>[%logger]</i>" : "[%logger]") +
                    "\u00A0%message%exceptionpadding{\n}%exception"
            };

            unityConsolePattern.AddConverter("exceptionpadding", typeof(ExceptionPaddingPatternConverter));
            unityConsolePattern.ActivateOptions();

            Appender = new UnityDebugLogAppender {
                Layout = unityConsolePattern
            };

            foreach (IFilter logsFilter in logFilters) {
                Appender.AddFilter(logsFilter);
            }

            /*
            // Don't double up our own logs
            Appender.AddFilter(new LoggerMatchFilter {
                LoggerToMatch = UnityLogger.Name,
                AcceptOnMatch = false
            });*/
        }

        public void StartUnityLogsProxying() {
            Application.logMessageReceivedThreaded += OnApplicationLogMessageReceivedThreaded;
        }

        public void Dispose() {
            Application.logMessageReceivedThreaded -= OnApplicationLogMessageReceivedThreaded;
        }

        private void OnApplicationLogMessageReceivedThreaded(string condition, string stacktrace, LogType type) {
            // Sends logs emitted by Unity or other third-party code to log4net
            if (IsOurOwnMessage(condition))
                return;

            LoggingEventData loggingEventData = new() {
                Level = type switch {
                    LogType.Error => Level.Error,
                    LogType.Assert => Level.Alert,
                    LogType.Warning => Level.Warn,
                    LogType.Log => Level.Info,
                    LogType.Exception => Level.Critical,
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                },
                LoggerName = UnityLogger.Name,
                Message = condition,
                TimeStampUtc = DateTime.UtcNow,
                Properties = new PropertiesDictionary {
                    [StackTraceUtility.LoggingEventStackTraceProperty] = stacktrace
                }
            };

            LoggingEvent loggingEvent = new(null, null, loggingEventData, FixFlags.None);
            try {
                UnityLogger.Repository.Log(loggingEvent);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        private bool IsOurOwnMessage(string message) {
            int patternStartIndex =
                message.IndexOf(CanUseRichText ? "<i>[" : "[", StringComparison.Ordinal);
            int patternEndIndex =
                message.IndexOf(CanUseRichText ? "]</i>\u00A0" : "]\u00A0", StringComparison.Ordinal);

            return patternStartIndex == 0 && patternEndIndex > patternStartIndex;
        }
    }
}
