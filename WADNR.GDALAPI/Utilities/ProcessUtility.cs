using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace WADNR.GDALAPI.Utilities;

public static class ProcessUtility
{
    private static readonly TimeSpan MaxTimeout = TimeSpan.FromMinutes(10);

    public static string ConjoinCommandLineArguments(List<string> commandLineArguments)
    {
        return string.Join(" ", commandLineArguments.Select(EncodeArgumentForCommandLine).ToList());
    }

    public static string ConjoinEnvironmentVariables(Dictionary<string, string> environmentVariables)
    {
        return string.Join("\r\n\t", environmentVariables.Select(x => $"{x.Key}: {x.Value}").ToList());
    }

    public static ProcessUtilityResult ShellAndWaitImpl(string? workingDirectory, string exeFileName, List<string> commandLineArguments, bool redirectStdErrAndStdOut, int? maxTimeoutMs, Dictionary<string, string> environmentVariables, ILogger logger)
    {
        var stdErrAndStdOut = string.Empty;

        var processStartInfo = new ProcessStartInfo(exeFileName);
        foreach (var arg in commandLineArguments)
        {
            processStartInfo.ArgumentList.Add(arg);
        }
        processStartInfo.UseShellExecute = false;

        if (environmentVariables != null && environmentVariables.Any())
        {
            foreach (var environmentVariable in environmentVariables)
            {
                processStartInfo.EnvironmentVariables[environmentVariable.Key] = environmentVariable.Value;
            }
        }
        var objProc = new Process { StartInfo = processStartInfo };

        if (!string.IsNullOrEmpty(workingDirectory))
        {
            objProc.StartInfo.WorkingDirectory = workingDirectory;
        }
        var streamReader = new ProcessStreamReader();
        if (redirectStdErrAndStdOut)
        {
            objProc.StartInfo.RedirectStandardOutput = true;
            objProc.StartInfo.RedirectStandardError = true;
            objProc.StartInfo.CreateNoWindow = true;
            objProc.OutputDataReceived += streamReader.ReceiveStdOut;
            objProc.ErrorDataReceived += streamReader.ReceiveStdErr;
        }

        var processDebugInfo = $"Process Details:\r\n\"{exeFileName}\" {ConjoinCommandLineArguments(commandLineArguments)}\r\nWorking Directory: {workingDirectory}\r\nEnvironment Variables: {ConjoinEnvironmentVariables(environmentVariables ?? new())}";
        logger.LogInformation($"Starting Process: {processDebugInfo}");
        try
        {
            objProc.Start();
        }
        catch (Exception e)
        {
            var message = $"Program {Path.GetFileName(exeFileName)} got an exception on process start.\r\nException message: {e.Message}\r\n{processDebugInfo}";
            throw new Exception(message, e);
        }

        if (redirectStdErrAndStdOut)
        {
            objProc.BeginOutputReadLine();
            objProc.BeginErrorReadLine();
        }

        var processTimeoutPeriod = maxTimeoutMs.HasValue ? TimeSpan.FromMilliseconds(maxTimeoutMs.Value) : MaxTimeout;
        var hasExited = objProc.WaitForExit(Convert.ToInt32(processTimeoutPeriod.TotalMilliseconds));

        if (!hasExited)
        {
            objProc.Kill();
        }

        if (redirectStdErrAndStdOut)
        {
            Thread.Sleep(TimeSpan.FromSeconds(.25));
            stdErrAndStdOut = streamReader.StdOutAndStdErr;
        }

        if (!hasExited)
        {
            var message = $"Program {Path.GetFileName(exeFileName)} did not exit within timeout period {processTimeoutPeriod} and was terminated.\r\n{processDebugInfo}\r\nOutput:\r\n{streamReader.StdOutAndStdErr}";
            throw new Exception(message);
        }

        return new ProcessUtilityResult(objProc.ExitCode, streamReader.StdOut, stdErrAndStdOut);
    }

    private class ProcessStreamReader
    {
        private readonly object _outputLock = new object();
        private readonly StringBuilder _diagnosticOutput = new();
        private readonly StringBuilder _standardOut = new();

        public void ReceiveStdOut(object sender, DataReceivedEventArgs e)
        {
            lock (_outputLock)
            {
                _diagnosticOutput.Append($"[stdout] {e.Data}\r\n");
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    _standardOut.Append($"{e.Data}\r\n");
                }
            }
        }

        public void ReceiveStdErr(object sender, DataReceivedEventArgs e)
        {
            var message = $"[stderr] {e.Data}";
            lock (_outputLock)
            {
                _diagnosticOutput.Append($"{message}\r\n");
            }
        }

        public string StdOutAndStdErr
        {
            get { lock (_outputLock) { return _diagnosticOutput.ToString(); } }
        }

        public string StdOut
        {
            get { lock (_outputLock) { return _standardOut.ToString(); } }
        }
    }

    public static string EncodeArgumentForCommandLine(string unencodedCommandLineArgument)
    {
        var charactersThatRequireEncodingRegex = new Regex("[ \t\r\n\v]");
        if (!charactersThatRequireEncodingRegex.IsMatch(unencodedCommandLineArgument))
        {
            return unencodedCommandLineArgument;
        }

        const char backslash = '\\';
        var encodedArgument = string.Empty;
        for (var i = 0; ; i++)
        {
            var numberOfBackslashes = 0;
            while (i < unencodedCommandLineArgument.Length && unencodedCommandLineArgument[i] == backslash)
            {
                ++i;
                ++numberOfBackslashes;
            }

            if (i == unencodedCommandLineArgument.Length)
            {
                encodedArgument += new string(backslash, numberOfBackslashes * 2);
                break;
            }

            if (unencodedCommandLineArgument[i] == '"')
            {
                encodedArgument += new string(backslash, numberOfBackslashes * 2 + 1);
                encodedArgument += unencodedCommandLineArgument[i];
            }
            else
            {
                encodedArgument += new string(backslash, numberOfBackslashes);
                encodedArgument += unencodedCommandLineArgument[i];
            }
        }
        return $"\"{encodedArgument}\"";
    }
}
