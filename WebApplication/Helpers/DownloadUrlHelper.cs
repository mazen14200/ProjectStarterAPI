using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Helpers
{
    public static class DownloadUrlHelper
    {
        public static string Build(IUrlHelper urlHelper, string? path, string? fileName = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "#";
            }

            return urlHelper.Action("Get", "Download", new
            {
                area = "",
                path = path.Replace("~", string.Empty),
                fileName = string.IsNullOrWhiteSpace(fileName) ? Path.GetFileName(path) : fileName
            }) ?? "#";
        }
    }
}
