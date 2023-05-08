using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Debug = UnityEngine.Debug;

namespace LostPolygon.Unity.Utility.Editor {
    public static class CommandExecutor {
        public static bool IsLinux =>
            Environment.OSVersion.Platform is PlatformID.Unix or PlatformID.MacOSX or (PlatformID) 128;

        public static bool Chmod(string filePath, string permissions = "700", bool recursive = false) {
            if (!IsLinux)
                return false;

            string cmd = recursive ?
                $"chmod -R {permissions} {filePath}" :
                $"chmod {permissions} {filePath}";

            try {
                using Process proc = Process.Start("/bin/sh", $"-c \"{cmd}\"");
                proc.WaitForExit();
                return proc.ExitCode == 0;
            } catch {
                return false;
            }
        }

        public static string ExecuteCommandWithOutput(string fileName, string arguments) {
            try {
                (int exitCode, string output) = ExecuteCommand(fileName, arguments, timeout: 20000);
                if (exitCode != 0)
                    throw new Exception($"exitCode == {exitCode}, output: {output}");

                return output;
            } catch (Exception e) {
                Debug.LogError($"Error while executing \"{fileName} {arguments}\"");
                Debug.LogException(e);
                throw;
            }
        }

        public static string ExecuteCommandWithOutputSafe(string fileName, string arguments, string onErrorValue = "[error]") {
            try {
                (int exitCode, string output) = ExecuteCommand(fileName, arguments, timeout: 20000);
                if (exitCode != 0)
                    throw new Exception("exitCode != 0");

                return output;
            } catch (Exception e) {
                Debug.LogError($"Error while executing \"{fileName} {arguments}\"");
                Debug.LogException(e);

                return onErrorValue;
            }
        }

        public static(int exitCode, string output) ExecuteCommand(
            string fileName,
            string arguments,
            string standardInput = null,
            Encoding standardInputEncoding = null,
            int timeout = 5000,
            Action<Process> modifyProcessCallback = null
        ) {
            Process process = new Process {
                StartInfo = {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = standardInput != null,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };

            modifyProcessCallback?.Invoke(process);

            process.Start();
            if (standardInput != null) {
                StreamWriter streamWriter = new StreamWriter(
                    process.StandardInput.BaseStream,
                    standardInputEncoding ?? new UTF8Encoding(false)
                );
                streamWriter.Write(standardInput);
                streamWriter.Close();
            }

            string result = process.StandardOutput.ReadToEnd().Trim();
            string resultError = process.StandardError.ReadToEnd().Trim();
            bool exited = process.WaitForExit(timeout);
            if (!exited) {
                if (!process.HasExited) {
                    process.Kill();
                }

                throw new TimeoutException($"'{fileName} {arguments}' did not finish in time, output: \r\n{result}");
            }

            string output = result;
            if (!String.IsNullOrWhiteSpace(resultError)) {
                output += "\n" + resultError;
            }

            return (process.ExitCode, output);
        }

        public static int ExecuteCommandWithStreamedOutput(
            string fileName,
            string arguments,
            DataReceivedEventHandler outputDataReceivedCallback,
            DataReceivedEventHandler errorDataReceivedCallback,
            string workingDirectory = null,
            int timeout = 5000
        ) {
            Process program = new Process {
                StartInfo = {
                    WorkingDirectory = workingDirectory ?? "",
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                },
                EnableRaisingEvents = true
            };

            program.OutputDataReceived += outputDataReceivedCallback;
            program.ErrorDataReceived += errorDataReceivedCallback;

            program.Start();

            program.BeginErrorReadLine();
            program.BeginOutputReadLine();

            bool exited = program.WaitForExit(timeout);
            if (!exited) {
                if (!program.HasExited) {
                    program.Kill();
                }

                throw new TimeoutException($"'{fileName} {arguments}' did not finish in time");
            }

            return program.ExitCode;
        }
    }
}
