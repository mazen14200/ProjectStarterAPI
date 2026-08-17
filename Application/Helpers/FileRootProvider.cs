using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Application.Helpers
{
    public static class FileRootProvider
    {
        private static string? _uploadsRootPath;

        public static void Configure(
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            var configuredPath =
                configuration.GetSection("UploadSettings")
                             .GetValue<string>("UploadsRootPath");

            _uploadsRootPath =
                !string.IsNullOrWhiteSpace(configuredPath)
                    ? Path.GetFullPath(configuredPath)
                    : Path.Combine(env.WebRootPath, "uploads");

            Directory.CreateDirectory(_uploadsRootPath);
        }

        public static string UploadsRootPath =>
            _uploadsRootPath
            ?? throw new InvalidOperationException(
                "FileRootProvider is not configured.");
    }
}
