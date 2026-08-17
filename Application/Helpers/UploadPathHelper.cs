namespace Application.Helpers
{
    public static class UploadPathHelper
    {
        public static string Root =>
            FileRootProvider.UploadsRootPath;

        public static string Combine(params string[] paths)
        {
            return Path.Combine(
                new[] { Root }.Concat(paths).ToArray());
        }

        public static string ResolveRelative(string relativePath)
        {
            relativePath = relativePath.Replace(
                '/',
                Path.DirectorySeparatorChar);

            if (relativePath.StartsWith(
                $"uploads{Path.DirectorySeparatorChar}",
                StringComparison.OrdinalIgnoreCase))
            {
                relativePath =
                    relativePath[(8)..];
            }

            return Combine(relativePath);
        }
    }
}
