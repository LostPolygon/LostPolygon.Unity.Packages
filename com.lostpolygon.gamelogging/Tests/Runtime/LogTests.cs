using System.Diagnostics;
using log4net;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;
using StackTraceUtility = LostPolygon.Log4netExtensions.StackTraceUtility;

namespace LostPolygon.Unity.GameLogging.Tests {
    public class LogTests {
        public GameLoggingFacade Facade { get; set; }

        [SetUp]
        public void SetUp() {
            Facade = new GameLoggingFacade("TestRepository")
                .AddUnityLogsProxying()
                .AddFileLog(
                    "TestLog.html",
                    "Test Log"
                )
                .Configure();
        }

        [TearDown]
        public void TearDown() {
            Facade?.Dispose();
        }

        [Test]
        public void BasicLogTest() {
            LogAssert.ignoreFailingMessages = true;

            ILog testLog = Facade.GetLog("Test");

            Debug.Log("Test 'Debug.Log' message");
            Debug.LogWarning("Test 'Debug.LogWarning' message");
            Debug.LogError("Test 'Debug.LogError' message");

            testLog.Debug("Test 'ILog.Debug' message");
            testLog.Info("Test 'ILog.Info' message");
            testLog.Warn("Test 'ILog.Warn' message");
            testLog.Error("Test 'ILog.Error' message");
            testLog.Fatal("Test 'ILog.Fatal' message");
        }

        [Test]
        public void AddLinksToStackTrace() {
            string formattedStackTrace = StackTraceUtility.AddLinksToStackTrace(@"UnityEngine.StackTraceUtility:ExtractStackTrace ()
VisionMove.Battle.StackTraceUtility:FormatStackTraceString (log4net.Core.LoggingEvent,bool&) (at Assets/Game/Source/Runtime/Library/Core/Log/log4NetExtensions/StackTraceUtility.cs:42)
VisionMove.Battle.StackTraceUtility:UpdateStackTrace (log4net.Core.LoggingEvent) (at Assets/Game/Source/Runtime/Library/Core/Log/log4NetExtensions/StackTraceUtility.cs:28)
log4net.Appender.AppenderSkeleton:DoAppend (log4net.Core.LoggingEvent)");

            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "{0}", formattedStackTrace);

            Assert.AreEqual(@"UnityEngine.StackTraceUtility:ExtractStackTrace ()
VisionMove.Battle.StackTraceUtility:FormatStackTraceString (log4net.Core.LoggingEvent,bool&) (at <a href=""Assets/Game/Source/Runtime/Library/Core/Log/log4NetExtensions/StackTraceUtility.cs"" line=""42"">Assets/Game/Source/Runtime/Library/Core/Log/log4NetExtensions/StackTraceUtility.cs:42</a>)
VisionMove.Battle.StackTraceUtility:UpdateStackTrace (log4net.Core.LoggingEvent) (at <a href=""Assets/Game/Source/Runtime/Library/Core/Log/log4NetExtensions/StackTraceUtility.cs"" line=""28"">Assets/Game/Source/Runtime/Library/Core/Log/log4NetExtensions/StackTraceUtility.cs:28</a>)
log4net.Appender.AppenderSkeleton:DoAppend (log4net.Core.LoggingEvent)", formattedStackTrace);
        }

        [Test]
        public void OneThousandLogs() {
            ILog testLog = Facade.GetLog("Test");

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++) {
                testLog.Debug($"Test 'ILog.Debug' message #{i}");
            }

            sw.Stop();

            Debug.Log($"Elapsed {sw.ElapsedMilliseconds} ms");
        }
    }
}
