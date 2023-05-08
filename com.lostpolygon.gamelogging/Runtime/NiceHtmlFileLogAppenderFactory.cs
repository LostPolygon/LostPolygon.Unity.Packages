using System.Collections.Generic;
using System.Text;
using log4net.Appender;
using log4net.Filter;
using LostPolygon.Log4netExtensions;
using LostPolygon.Unity.Log4net;
using UnityEngine;

namespace LostPolygon.Unity.GameLogging {
    public static class NiceHtmlFileLogAppenderFactory {
        public static IReadOnlyList<IAppender> CreateAppenders(
            string logFilePath,
            string logTitle,
            IEnumerable<IFilter> logFilters
        ) {
            HtmlLayout htmlLayout = new NiceHtmlLayout("%counter%utcdate{HH:mm:ss}%level%logger%message");

            htmlLayout.LogName = logTitle;
            htmlLayout.ActivateOptions();

            RollingFileAppender fileAppender = new() {
                File = logFilePath,
                Layout = htmlLayout,
                Encoding = Encoding.UTF8,
                RollingStyle = RollingFileAppender.RollingMode.Once,
                MaxSizeRollBackups = Application.isBatchMode ? 0 : 3,
                PreserveLogFileNameExtension = true
            };

            foreach (IFilter logsFilter in logFilters) {
                fileAppender.AddFilter(logsFilter);
            }

            fileAppender.ActivateOptions();

            // Avoid using log4net's stacktrace generation facilities
            // and instead pre-fill stack trace with Unity
            AddUnityStackTraceAppender unityStackTraceAppender =
                new("log4net.Repository.Hierarchy.Logger:Log (System.Type,log4net.Core.Level,object,System.Exception)\n");

            return new IAppender[] {
                unityStackTraceAppender,
                fileAppender
            };
        }
    }
}
