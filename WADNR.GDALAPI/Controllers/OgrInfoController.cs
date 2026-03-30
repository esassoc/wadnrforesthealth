using System.IO.Compression;
using WADNR.GDALAPI.Models;
using WADNR.GDALAPI.Services;
using WADNR.GDALAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace WADNR.GDALAPI.Controllers;

[ApiController]
public class OgrInfoController : ControllerBase
{
    private readonly ILogger<OgrInfoController> _logger;
    private readonly OgrInfoService _ogrInfoService;

    public OgrInfoController(ILogger<OgrInfoController> logger, OgrInfoService ogrInfoService)
    {
        _logger = logger;
        _ogrInfoService = ogrInfoService;
    }

    [HttpPost("ogrinfo/gdb-feature-classes")]
    [RequestSizeLimit(10_000_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10_000_000_000)]
    public async Task<ActionResult<List<FeatureClassInfo>>> GdbToFeatureClassInfo([FromForm] IFormFile file)
    {
        using var disposableTempGdbZipFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".gdb.zip");

        await using (var fileStream = new FileStream(disposableTempGdbZipFile.FileInfo.FullName, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        var args = BuildOgrInfoCommandLineArgumentsToListFeatureClassInfos(disposableTempGdbZipFile.FileInfo.FullName);

        try
        {
            var processUtilityResult = _ogrInfoService.Run(args);
            var stdOutString = processUtilityResult.StdOut;
            var featureClassesFromFileGdb = stdOutString.Split(new[] { "\r\nLayer name: " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
            var featureClassInfos = new List<FeatureClassInfo>();
            foreach (var featureClassBlob in featureClassesFromFileGdb)
            {
                var featureClassInfo = new FeatureClassInfo();
                var features = featureClassBlob.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                featureClassInfo.LayerName = features.First();
                featureClassInfo.FeatureType = features.First(x => x.StartsWith("Geometry: ")).Substring("Geometry: ".Length);
                featureClassInfo.FeatureCount = int.Parse(features.First(x => x.StartsWith("Feature Count: ")).Substring("Feature Count: ".Length));

                var columnNamesBlob = featureClassBlob.Split(new[] { "FID Column = " }, StringSplitOptions.RemoveEmptyEntries);
                if (columnNamesBlob.Length == 2)
                {
                    featureClassInfo.Columns = columnNamesBlob.Skip(1).Single()
                        .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => !x.StartsWith("Geometry Column"))
                        .Select(x => x.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries).First())
                        .ToList();
                }
                else
                {
                    featureClassInfo.Columns = new List<string>();
                }

                featureClassInfos.Add(featureClassInfo);
            }

            return Ok(featureClassInfos);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing GDB file");
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("ogrinfo/shp-feature-classes")]
    [RequestSizeLimit(10_000_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10_000_000_000)]
    public async Task<ActionResult<List<FeatureClassInfo>>> ShpToFeatureClassInfo([FromForm] IFormFile file)
    {
        var extractDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        try
        {
            // Save zip and extract to temp directory so GDAL can read the .shp files directly
            var tempZipPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");
            await using (var fileStream = new FileStream(tempZipPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            ZipFile.ExtractToDirectory(tempZipPath, extractDir);
            System.IO.File.Delete(tempZipPath);

            // Find all .shp files in the extracted directory and run ogrinfo on each
            var shpFiles = Directory.GetFiles(extractDir, "*.shp", SearchOption.AllDirectories);
            var featureClassInfos = new List<FeatureClassInfo>();

            foreach (var shpFile in shpFiles)
            {
                var args = BuildOgrInfoCommandLineArgumentsToListFeatureClassInfos(shpFile);
                var processUtilityResult = _ogrInfoService.Run(args);
                var stdOutString = processUtilityResult.StdOut;
                var featureClassesFromShp = stdOutString.Split(new[] { "\r\nLayer name: " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();

                foreach (var featureClassBlob in featureClassesFromShp)
                {
                    var featureClassInfo = new FeatureClassInfo();
                    var features = featureClassBlob.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    featureClassInfo.LayerName = features.First();
                    featureClassInfo.FeatureType = features.First(x => x.StartsWith("Geometry: ")).Substring("Geometry: ".Length);
                    featureClassInfo.FeatureCount = int.Parse(features.First(x => x.StartsWith("Feature Count: ")).Substring("Feature Count: ".Length));

                    var columnNamesBlob = featureClassBlob.Split(new[] { "FID Column = " }, StringSplitOptions.RemoveEmptyEntries);
                    if (columnNamesBlob.Length == 2)
                    {
                        featureClassInfo.Columns = columnNamesBlob.Skip(1).Single()
                            .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(x => !x.StartsWith("Geometry Column"))
                            .Select(x => x.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries).First())
                            .ToList();
                    }
                    else
                    {
                        featureClassInfo.Columns = new List<string>();
                    }

                    featureClassInfos.Add(featureClassInfo);
                }
            }

            return Ok(featureClassInfos);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing shapefile zip");
            return StatusCode(500, e.Message);
        }
        finally
        {
            if (Directory.Exists(extractDir))
            {
                Directory.Delete(extractDir, true);
            }
        }
    }

    private static List<string> BuildOgrInfoCommandLineArgumentsToListFeatureClassInfos(string inputGdbFile)
    {
        return new List<string> { "-al", "-ro", "-so", "-noextent", inputGdbFile };
    }
}
