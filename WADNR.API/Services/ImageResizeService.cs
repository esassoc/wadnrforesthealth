using System;
using System.IO;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace WADNR.API.Services
{
    public class ImageResizeService
    {
        public const long MaxStoredImageBytes = 5L * 1024 * 1024;
        private const int InitialMaxLongestEdgePx = 2000;
        private const int InitialJpegQuality = 85;
        private const int MinJpegQuality = 50;
        private const int JpegQualityStep = 10;
        private const int MinLongestEdgePx = 500;

        private readonly ILogger<ImageResizeService> _logger;

        public ImageResizeService(ILogger<ImageResizeService> logger)
        {
            _logger = logger;
        }

        public ResizeResult ResizeIfNeeded(Stream input, string originalExtension)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (input.CanSeek) input.Seek(0, SeekOrigin.Begin);

            var format = MapExtensionToFormat(originalExtension);
            if (!format.HasValue)
            {
                return ResizeResult.Invalid("Unsupported image extension.");
            }

            var buffered = CopyToMemoryStream(input);
            var originalLength = buffered.Length;

            if (originalLength <= MaxStoredImageBytes)
            {
                buffered.Seek(0, SeekOrigin.Begin);
                return ResizeResult.Ok(buffered, originalExtension, originalLength);
            }

            buffered.Seek(0, SeekOrigin.Begin);
            using var bitmap = SKBitmap.Decode(buffered);
            if (bitmap == null)
            {
                buffered.Dispose();
                return ResizeResult.Invalid("File could not be decoded as an image.");
            }

            // Animated GIFs lose animation here — SkiaSharp re-encodes only the first frame.
            // Acceptable trade-off for project photos; the modal accepts gif/jpg/jpeg/png.
            var maxEdge = InitialMaxLongestEdgePx;
            for (var dimensionAttempt = 0; dimensionAttempt < 3; dimensionAttempt++)
            {
                using var resized = ResizeBitmap(bitmap, maxEdge);
                using var image = SKImage.FromBitmap(resized);

                var quality = InitialJpegQuality;
                while (true)
                {
                    using var encoded = image.Encode(format.Value, quality);
                    if (encoded == null)
                    {
                        buffered.Dispose();
                        return ResizeResult.Invalid("Image encoding failed.");
                    }

                    if (encoded.Size <= MaxStoredImageBytes)
                    {
                        buffered.Dispose();
                        var output = new MemoryStream(checked((int)encoded.Size));
                        encoded.SaveTo(output);
                        output.Seek(0, SeekOrigin.Begin);
                        _logger.LogInformation(
                            "Resized image from {OriginalBytes} to {NewBytes} (longest edge {Edge}px, quality {Quality}).",
                            originalLength, output.Length, maxEdge, quality);
                        return ResizeResult.Ok(output, originalExtension, output.Length);
                    }

                    if (format.Value != SKEncodedImageFormat.Jpeg || quality <= MinJpegQuality)
                    {
                        break;
                    }
                    quality -= JpegQualityStep;
                }

                if (maxEdge <= MinLongestEdgePx) break;
                maxEdge = Math.Max(MinLongestEdgePx, maxEdge / 2);
            }

            buffered.Dispose();
            return ResizeResult.Invalid("Image could not be reduced under the 5MB limit.");
        }

        private static SKBitmap ResizeBitmap(SKBitmap source, int maxLongestEdgePx)
        {
            var longest = Math.Max(source.Width, source.Height);
            if (longest <= maxLongestEdgePx)
            {
                return source.Copy();
            }

            var scale = (double)maxLongestEdgePx / longest;
            var newWidth = Math.Max(1, (int)Math.Round(source.Width * scale));
            var newHeight = Math.Max(1, (int)Math.Round(source.Height * scale));
            var info = new SKImageInfo(newWidth, newHeight, source.ColorType, source.AlphaType);
            var resized = source.Resize(info, SKSamplingOptions.Default);
            return resized ?? source.Copy();
        }

        private static MemoryStream CopyToMemoryStream(Stream input)
        {
            var ms = new MemoryStream();
            input.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private static SKEncodedImageFormat? MapExtensionToFormat(string extension)
        {
            if (string.IsNullOrEmpty(extension)) return null;
            var ext = extension.StartsWith('.') ? extension.Substring(1) : extension;
            return ext.ToLowerInvariant() switch
            {
                "jpg" => SKEncodedImageFormat.Jpeg,
                "jpeg" => SKEncodedImageFormat.Jpeg,
                "png" => SKEncodedImageFormat.Png,
                "gif" => SKEncodedImageFormat.Gif,
                _ => null
            };
        }
    }

    public class ResizeResult
    {
        public bool IsValid { get; }
        public Stream Stream { get; }
        public string Extension { get; }
        public long Length { get; }
        public string ErrorMessage { get; }

        private ResizeResult(bool isValid, Stream stream, string extension, long length, string errorMessage)
        {
            IsValid = isValid;
            Stream = stream;
            Extension = extension;
            Length = length;
            ErrorMessage = errorMessage;
        }

        public static ResizeResult Ok(Stream stream, string extension, long length) =>
            new(true, stream, extension, length, null);

        public static ResizeResult Invalid(string message) =>
            new(false, null, null, 0, message);
    }
}
