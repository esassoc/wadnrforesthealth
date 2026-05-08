using Microsoft.Extensions.Logging.Abstractions;
using SkiaSharp;
using WADNR.API.Services;

namespace WADNR.API.Tests.Services;

[TestClass]
public class ImageResizeServiceTests
{
    private static ImageResizeService CreateService() =>
        new(NullLogger<ImageResizeService>.Instance);

    [TestMethod]
    public void ResizeIfNeeded_ReturnsCopy_WhenAlreadyUnderLimit()
    {
        var service = CreateService();
        using var input = MakeNoiseImage(100, 100, SKEncodedImageFormat.Jpeg, quality: 80);
        Assert.IsTrue(input.Length < ImageResizeService.MaxStoredImageBytes);

        var result = service.ResizeIfNeeded(input, ".jpg");

        Assert.IsTrue(result.IsValid, result.ErrorMessage);
        Assert.AreEqual(".jpg", result.Extension);
        Assert.AreEqual(input.Length, result.Length);
        Assert.IsTrue(result.Stream.CanRead);
        result.Stream.Dispose();
    }

    [TestMethod]
    public void ResizeIfNeeded_ResizesLargeJpeg_ToUnderLimit()
    {
        var service = CreateService();
        using var input = MakeNoiseImage(4000, 4000, SKEncodedImageFormat.Jpeg, quality: 100);
        Assert.IsTrue(input.Length > ImageResizeService.MaxStoredImageBytes,
            $"Test fixture should be > 5MB but was {input.Length} bytes");

        var result = service.ResizeIfNeeded(input, ".jpg");

        Assert.IsTrue(result.IsValid, result.ErrorMessage);
        Assert.AreEqual(".jpg", result.Extension);
        Assert.IsTrue(result.Length <= ImageResizeService.MaxStoredImageBytes,
            $"Resized output was {result.Length} bytes, exceeds 5MB cap");
        AssertStreamIsDecodableImage(result.Stream);
    }

    [TestMethod]
    public void ResizeIfNeeded_ResizesLargePng_ToUnderLimit()
    {
        var service = CreateService();
        using var input = MakeNoiseImage(1800, 1800, SKEncodedImageFormat.Png, quality: 100);
        Assert.IsTrue(input.Length > ImageResizeService.MaxStoredImageBytes,
            $"Test fixture should be > 5MB but was {input.Length} bytes");

        var result = service.ResizeIfNeeded(input, ".png");

        Assert.IsTrue(result.IsValid, result.ErrorMessage);
        Assert.AreEqual(".png", result.Extension);
        Assert.IsTrue(result.Length <= ImageResizeService.MaxStoredImageBytes,
            $"Resized output was {result.Length} bytes, exceeds 5MB cap");
        AssertStreamIsDecodableImage(result.Stream);
    }

    [TestMethod]
    public void ResizeIfNeeded_ReturnsInvalid_ForGarbageBytes()
    {
        var service = CreateService();
        var garbage = new byte[ImageResizeService.MaxStoredImageBytes + 1024];
        new Random(42).NextBytes(garbage);
        using var input = new MemoryStream(garbage);

        var result = service.ResizeIfNeeded(input, ".jpg");

        Assert.IsFalse(result.IsValid);
        Assert.IsNull(result.Stream);
    }

    [TestMethod]
    public void ResizeIfNeeded_ReturnsInvalid_ForUnsupportedExtension()
    {
        var service = CreateService();
        using var input = MakeNoiseImage(100, 100, SKEncodedImageFormat.Jpeg, quality: 80);

        var result = service.ResizeIfNeeded(input, ".heic");

        Assert.IsFalse(result.IsValid);
    }

    [TestMethod]
    public void ResizeIfNeeded_PreservesExtension_ForJpegVariants()
    {
        var service = CreateService();
        using var input = MakeNoiseImage(100, 100, SKEncodedImageFormat.Jpeg, quality: 80);

        var result = service.ResizeIfNeeded(input, ".jpeg");

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(".jpeg", result.Extension);
        result.Stream.Dispose();
    }

    private static MemoryStream MakeNoiseImage(int width, int height, SKEncodedImageFormat format, int quality)
    {
        using var bitmap = new SKBitmap(width, height, isOpaque: true);
        var rand = new Random(12345);
        var pixels = new byte[width * height * 4];
        rand.NextBytes(pixels);
        // Force alpha to opaque so JPEG/PNG round-trip cleanly.
        for (var i = 3; i < pixels.Length; i += 4) pixels[i] = 255;
        System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bitmap.GetPixels(), pixels.Length);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(format, quality);
        var ms = new MemoryStream();
        data.SaveTo(ms);
        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }

    private static void AssertStreamIsDecodableImage(Stream stream)
    {
        Assert.IsNotNull(stream);
        stream.Seek(0, SeekOrigin.Begin);
        using var bitmap = SKBitmap.Decode(stream);
        Assert.IsNotNull(bitmap, "Resized output is not a decodable image.");
        Assert.IsTrue(bitmap.Width > 0 && bitmap.Height > 0);
    }
}
