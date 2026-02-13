using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services;

public class GDALAPIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GDALAPIService> _logger;

    public GDALAPIService(ILogger<GDALAPIService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<List<GdbFeatureClassPreview>> OgrInfoGdbToFeatureClassInfo(IFormFile formFile)
    {
        using var ms = new MemoryStream();
        await formFile.CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);
        var byteContent = new StreamContent(ms);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);

        var form = new MultipartFormDataContent();
        form.Add(byteContent, "file", formFile.FileName);

        _logger.LogInformation("Sending ogrinfo request to GDAL API");

        var response = await _httpClient.PostAsync("/ogrinfo/gdb-feature-classes", form);
        if (response.IsSuccessStatusCode)
        {
            var featureClassInfos = await response.Content.ReadFromJsonAsync<List<FeatureClassInfoResponse>>();
            return featureClassInfos?.Select(f => new GdbFeatureClassPreview
            {
                FeatureClassName = f.LayerName,
                FeatureType = f.FeatureType,
                FeatureCount = f.FeatureCount,
                PropertyNames = f.Columns
            }).ToList() ?? new List<GdbFeatureClassPreview>();
        }

        var content = await response.Content.ReadAsStringAsync();
        throw new Exception($"GDAL API ogrinfo request failed: {content}");
    }

    public async Task<string> Ogr2OgrGdbLayerToGeoJson(IFormFile formFile, string featureClassName)
    {
        using var ms = new MemoryStream();
        await formFile.CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);
        var byteContent = new StreamContent(ms);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);

        var form = new MultipartFormDataContent();
        form.Add(byteContent, "file", formFile.FileName);
        form.Add(new StringContent(featureClassName), "featureClassName");

        _logger.LogInformation("Sending ogr2ogr request to GDAL API for layer {LayerName}", featureClassName);

        var response = await _httpClient.PostAsync("/ogr2ogr/gdb-to-geojson", form);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        var content = await response.Content.ReadAsStringAsync();
        throw new Exception($"GDAL API ogr2ogr request failed: {content}");
    }

    public async Task<List<GdbFeatureClassPreview>> OgrInfoShpToFeatureClassInfo(IFormFile formFile)
    {
        using var ms = new MemoryStream();
        await formFile.CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);
        var byteContent = new StreamContent(ms);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);

        var form = new MultipartFormDataContent();
        form.Add(byteContent, "file", formFile.FileName);

        _logger.LogInformation("Sending ogrinfo shapefile request to GDAL API");

        var response = await _httpClient.PostAsync("/ogrinfo/shp-feature-classes", form);
        if (response.IsSuccessStatusCode)
        {
            var featureClassInfos = await response.Content.ReadFromJsonAsync<List<FeatureClassInfoResponse>>();
            return featureClassInfos?.Select(f => new GdbFeatureClassPreview
            {
                FeatureClassName = f.LayerName,
                FeatureType = f.FeatureType,
                FeatureCount = f.FeatureCount,
                PropertyNames = f.Columns
            }).ToList() ?? new List<GdbFeatureClassPreview>();
        }

        var content = await response.Content.ReadAsStringAsync();
        throw new Exception($"GDAL API ogrinfo shapefile request failed: {content}");
    }

    public async Task<string> Ogr2OgrShpLayerToGeoJson(IFormFile formFile, string featureClassName)
    {
        using var ms = new MemoryStream();
        await formFile.CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);
        var byteContent = new StreamContent(ms);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);

        var form = new MultipartFormDataContent();
        form.Add(byteContent, "file", formFile.FileName);
        form.Add(new StringContent(featureClassName), "featureClassName");

        _logger.LogInformation("Sending ogr2ogr shapefile request to GDAL API for layer {LayerName}", featureClassName);

        var response = await _httpClient.PostAsync("/ogr2ogr/shp-to-geojson", form);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        var content = await response.Content.ReadAsStringAsync();
        throw new Exception($"GDAL API ogr2ogr shapefile request failed: {content}");
    }

    /// <summary>
    /// Internal DTO matching the GDAL API's FeatureClassInfo response shape.
    /// </summary>
    private class FeatureClassInfoResponse
    {
        public string LayerName { get; set; } = string.Empty;
        public string FeatureType { get; set; } = string.Empty;
        public int FeatureCount { get; set; }
        public List<string> Columns { get; set; } = new();
    }
}
