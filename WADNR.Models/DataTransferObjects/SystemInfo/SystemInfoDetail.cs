using System.Diagnostics;
using System.Reflection;

namespace WADNR.Models.DataTransferObjects;
public class SystemInfoDetail
{
    public string Environment { get; set; }
    public string CurrentTimeUTC { get; set; }
    public string Application => Assembly.GetEntryAssembly().GetName().Name;
    public string FullInformationalVersion { get; set; }
    public string PodName { get; set; }
    public string Version { get; set; }
    public DateTimeOffset CompilationDateTime { get; set; }

    public SystemInfoDetail()
    {
        FullInformationalVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        var assemblyVersion = Assembly.GetEntryAssembly()!.GetName().Version;
        Version = $"{assemblyVersion!.Major}.{assemblyVersion!.Minor}.{assemblyVersion!.Build}";

        var localAssemblyPathString = new Uri(Assembly.GetExecutingAssembly().Location).LocalPath;
        var fileInfo = new FileInfo(localAssemblyPathString);
        CompilationDateTime = fileInfo.LastWriteTime;
    }
}