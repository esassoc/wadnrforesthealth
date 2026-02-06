using WADNR.GDALAPI.Utilities;

namespace WADNR.GDALAPI.Services;

public class OgrInfoService
{
    private readonly ILogger<OgrInfoService> _logger;

    public OgrInfoService(ILogger<OgrInfoService> logger)
    {
        _logger = logger;
    }

    public ProcessUtilityResult Run(List<string> arguments)
    {
        var exeFileName = "ogrinfo";
        var processUtilityResult = ProcessUtility.ShellAndWaitImpl(null, exeFileName, arguments, true, 250000000, new Dictionary<string, string>(), _logger);
        if (processUtilityResult.ReturnCode != 0)
        {
            var argumentsAsString = string.Join(" ", arguments.Select(ProcessUtility.EncodeArgumentForCommandLine).ToList());
            var fullProcessAndArguments = $"{ProcessUtility.EncodeArgumentForCommandLine(exeFileName)} {argumentsAsString}";
            var errorMessage = $"Process \"{exeFileName}\" returned with exit code {processUtilityResult.ReturnCode}, expected exit code 0.\r\n\r\nStdErr and StdOut:\r\n{processUtilityResult.StdOutAndStdErr}\r\n\r\nProcess Command Line:\r\n{fullProcessAndArguments}";
            throw new ApplicationException(errorMessage);
        }
        return processUtilityResult;
    }
}
