using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LostPolygon.Unity.Utility.Editor {
    public static class BuildUtility {
        private static readonly string Eol = Environment.NewLine;

        public static void ExitWithResult(BuildResult result, bool exitOnSuccess) {
            int returnValue;
            switch (result) {
                case BuildResult.Succeeded:
                    Log("Build succeeded!");
                    returnValue = 0;
                    break;
                case BuildResult.Failed:
                    Log("Build failed!");
                    returnValue = 101;
                    break;
                case BuildResult.Cancelled:
                    Log("Build cancelled!");
                    returnValue = 102;
                    break;
                case BuildResult.Unknown:
                default:
                    Log("Build result is unknown!");
                    returnValue = 103;
                    break;
            }

            if (Application.isBatchMode && (exitOnSuccess || returnValue != 0)) {
                Log($"Exiting Editor with code {returnValue}");
                EditorApplication.Exit(returnValue);
            }
        }

        public static BuildSummary BuildAndReport(BuildPlayerOptions buildPlayerOptions, bool exitOnSuccess) {
            Log($"Starting build for target {buildPlayerOptions.target}, exitOnSuccess: {exitOnSuccess}");

            BuildSummary buildSummary = BuildPipeline.BuildPlayer(buildPlayerOptions).summary;
            BuildSummaryReport(buildSummary);
            ExitWithResult(buildSummary.result, exitOnSuccess);

            return buildSummary;
        }

        public static void BuildSummaryReport(BuildSummary summary) {
            Log(
                $"{Eol}" +
                $"###########################{Eol}" +
                $"#      Build results      #{Eol}" +
                $"###########################{Eol}" +
                $"{Eol}" +
                $"Duration: {summary.totalTime.ToString()}{Eol}" +
                $"Warnings: {summary.totalWarnings.ToString()}{Eol}" +
                $"Errors: {summary.totalErrors.ToString()}{Eol}" +
                $"Size: {summary.totalSize.ToString()} bytes{Eol}" +
                $"{Eol}"
            );
        }

        public static void AssertBuildTarget(BuildTarget expected) {
            if (EditorUserBuildSettings.activeBuildTarget != expected)
                throw new Exception(
                    $"Current build target is {EditorUserBuildSettings.activeBuildTarget}," +
                    $" expected {expected}"
                );
        }

        public static int ExecuteCommandWithStreamedOutput(
            string fileName,
            string arguments,
            string workingDirectory = null,
            bool logToUnityConsole = false,
            int timeout = 5000
        ) {
            void LogOutput(object sender, DataReceivedEventArgs args) {
                Console.WriteLine(args.Data);
                if (logToUnityConsole) {
                    Log(args.Data);
                }
            }

            void LogError(object sender, DataReceivedEventArgs args) {
                Console.WriteLine(args.Data);
                if (logToUnityConsole) {
                    Log(args.Data, true);
                }
            }

            return CommandExecutor.ExecuteCommandWithStreamedOutput(
                fileName,
                arguments,
                LogOutput,
                LogError,
                workingDirectory,
                timeout
            );
        }

        public static void Log(string message, bool error = false) {
            message = $"[{nameof(BuildUtility)}] {message}";
            if (error) {
                Debug.LogError(message);
            } else {
                Debug.Log(message);
            }
        }
    }
}
