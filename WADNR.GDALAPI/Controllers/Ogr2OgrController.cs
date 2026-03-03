using System.IO.Compression;
using System.Text.RegularExpressions;
using WADNR.GDALAPI.Services;
using WADNR.GDALAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace WADNR.GDALAPI.Controllers;

[ApiController]
public class Ogr2OgrController : ControllerBase
{
    private readonly ILogger<Ogr2OgrController> _logger;
    private readonly Ogr2OgrService _ogr2OgrService;

    private static readonly Regex ValidLayerNameRegex = new(@"^[\w\-. ]+$", RegexOptions.Compiled);

    public Ogr2OgrController(ILogger<Ogr2OgrController> logger, Ogr2OgrService ogr2OgrService)
    {
        _logger = logger;
        _ogr2OgrService = ogr2OgrService;
    }

    [HttpGet("/")]
    public ActionResult Get()
    {
        return Ok("Hello from the WADNR GDAL API!");
    }

    [HttpPost("ogr2ogr/gdb-to-geojson")]
    [RequestSizeLimit(10_000_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10_000_000_000)]
    public async Task<ActionResult<string>> GdbLayerToGeoJson([FromForm] IFormFile file, [FromForm] string featureClassName)
    {
        if (string.IsNullOrWhiteSpace(featureClassName) || !ValidLayerNameRegex.IsMatch(featureClassName))
        {
            return BadRequest("Invalid feature class name.");
        }

        using var disposableTempGdbZipFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".gdb.zip");

        await using (var fileStream = new FileStream(disposableTempGdbZipFile.FileInfo.FullName, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        try
        {
            var args = BuildCommandLineArgumentsForFileGdbToGeoJson(
                disposableTempGdbZipFile.FileInfo.FullName,
                featureClassName);

            var result = _ogr2OgrService.Run(args);
            return Ok(result.StdOut);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting GDB to GeoJSON");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("ogr2ogr/shp-to-geojson")]
    [RequestSizeLimit(10_000_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10_000_000_000)]
    public async Task<ActionResult<string>> ShpLayerToGeoJson([FromForm] IFormFile file, [FromForm] string featureClassName)
    {
        if (string.IsNullOrWhiteSpace(featureClassName) || !ValidLayerNameRegex.IsMatch(featureClassName))
        {
            return BadRequest("Invalid feature class name.");
        }

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

            // Find the .shp file matching the requested feature class name
            var shpFiles = Directory.GetFiles(extractDir, "*.shp", SearchOption.AllDirectories);
            var targetShp = shpFiles.FirstOrDefault(f =>
                Path.GetFileNameWithoutExtension(f).Equals(featureClassName, StringComparison.OrdinalIgnoreCase));

            if (targetShp == null)
            {
                return BadRequest($"Shapefile '{featureClassName}' not found in zip archive.");
            }

            var args = BuildCommandLineArgumentsForShpToGeoJson(targetShp, featureClassName);

            var result = _ogr2OgrService.Run(args);
            return Ok(result.StdOut);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting shapefile to GeoJSON");
            return StatusCode(500, ex.Message);
        }
        finally
        {
            if (Directory.Exists(extractDir))
            {
                Directory.Delete(extractDir, true);
            }
        }
    }

    [HttpPost("ogr2ogr/geojson-to-gdb")]
    [RequestSizeLimit(10_000_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10_000_000_000)]
    public async Task<IActionResult> GeoJsonToGdb([FromForm] IFormFile file, [FromForm] string layerName, [FromForm] string gdbName = null)
    {
        if (string.IsNullOrWhiteSpace(layerName) || !ValidLayerNameRegex.IsMatch(layerName))
        {
            return BadRequest("Invalid layer name.");
        }

        using var disposableGeoJsonFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".geojson");
        await using (var fileStream = new FileStream(disposableGeoJsonFile.FileInfo.FullName, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        var gdbDirName = !string.IsNullOrWhiteSpace(gdbName)
            ? string.Join("_", gdbName.Split(Path.GetInvalidFileNameChars())) + ".gdb"
            : Path.GetRandomFileName() + ".gdb";
        var outputGdbDir = Path.Combine(Path.GetTempPath(), gdbDirName);

        try
        {
            var args = BuildCommandLineArgumentsForGeoJsonToFileGdb(
                disposableGeoJsonFile.FileInfo.FullName,
                outputGdbDir,
                layerName);

            _ogr2OgrService.Run(args);

            if (!Directory.Exists(outputGdbDir))
            {
                return StatusCode(500, "ogr2ogr did not produce output GDB directory.");
            }

            // Zip the .gdb directory
            var zipPath = outputGdbDir + ".zip";
            ZipFile.CreateFromDirectory(outputGdbDir, zipPath, CompressionLevel.Optimal, true);

            var zipBytes = await System.IO.File.ReadAllBytesAsync(zipPath);
            System.IO.File.Delete(zipPath);

            return File(zipBytes, "application/zip", Path.GetFileName(zipPath));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting GeoJSON to GDB");
            return StatusCode(500, ex.Message);
        }
        finally
        {
            if (Directory.Exists(outputGdbDir))
            {
                Directory.Delete(outputGdbDir, true);
            }
        }
    }

    private static List<string> BuildCommandLineArgumentsForGeoJsonToFileGdb(string inputGeoJsonPath, string outputGdbPath, string layerName)
    {
        return new List<string>
        {
            "-f",
            "OpenFileGDB",
            outputGdbPath,
            inputGeoJsonPath,
            "-nln",
            layerName,
            "-t_srs",
            "EPSG:4326"
        };
    }

    private static List<string> BuildCommandLineArgumentsForFileGdbToGeoJson(string inputGdbFilePath, string sourceLayerName)
    {
        var commandLineArguments = new List<string>
        {
            "-sql",
            $"select * from \"{sourceLayerName}\"",
            "-t_srs",
            "EPSG:4326",
            "-f",
            "GeoJSON",
            "/dev/stdout",
            inputGdbFilePath,
            "-nln",
            sourceLayerName
        };

        return commandLineArguments;
    }

    private static List<string> BuildCommandLineArgumentsForShpToGeoJson(string inputShpZipPath, string sourceLayerName)
    {
        var commandLineArguments = new List<string>
        {
            "-sql",
            $"select * from \"{sourceLayerName}\"",
            "-t_srs",
            "EPSG:4326",
            "-f",
            "GeoJSON",
            "/dev/stdout",
            inputShpZipPath,
            "-nln",
            sourceLayerName
        };

        return commandLineArguments;
    }
}
