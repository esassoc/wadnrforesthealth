using WADNR.GDALAPI.Services;
using WADNR.GDALAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace WADNR.GDALAPI.Controllers;

[ApiController]
public class Ogr2OgrController : ControllerBase
{
    private readonly ILogger<Ogr2OgrController> _logger;
    private readonly Ogr2OgrService _ogr2OgrService;

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

    private static List<string> BuildCommandLineArgumentsForFileGdbToGeoJson(string inputGdbFilePath, string sourceLayerName)
    {
        var commandLineArguments = new List<string>
        {
            "-sql",
            $"select * from {sourceLayerName}",
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
}
