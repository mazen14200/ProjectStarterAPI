using Domain.Resources;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Application.Helpers
{
    public static class FileHelper
    {
        private static readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

        private static bool IsExtensionAllowed(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return _allowedExtensions.Contains(extension);
        }

        private static bool IsFileContentValid(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            using var stream = file.OpenReadStream();
            var reader = new BinaryReader(stream);
            byte[] header;

            if (extension == ".pdf")
            {
                header = reader.ReadBytes(5);
                return header.SequenceEqual(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }); // %PDF-
            }
            if (extension == ".jpg" || extension == ".jpeg")
            {
                header = reader.ReadBytes(3);
                return header.SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF }); // JPEG header
            }
            if (extension == ".png")
            {
                header = reader.ReadBytes(8);
                return header.SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }); // PNG header
            }

            return false;
        }

        public static async Task<string> SaveImageAsync(IFormFile file, string folderName)
        {
            if (file == null || string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentException("Invalid file or folder name");

            if (!IsExtensionAllowed(file.FileName))
                throw new InvalidOperationException("File extension not allowed.");

            if (!IsFileContentValid(file))
                throw new InvalidOperationException("File content does not match its extension.");

            var folder = UploadPathHelper.Combine(folderName);
            Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("uploads", folderName, fileName).Replace("\\", "/");
        }

        public static async Task<string> CheckFileIsPdf_5Mg_Async(IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                #region Only PDF files are allowed
                bool isPdf = true;
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".pdf")
                {
                    isPdf = false;
                    return Resource1.PdfOnly;
                }

                // If you want to make sure of the size, for example (5 MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    isPdf = false;
                    return Resource1.pdfFileMore5mg;
                }

                return "OK";
                #endregion
            }
            else
            {
                return "null";
            }
        }

        public static async Task<string> CheckFileIsImage_3Mg_Async(IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                #region Only Image files are allowed
                bool isPdf = true;
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    isPdf = false;
                    return Resource1.ImageOnly;
                    //ModelState.AddModelError("PdfFilePath", "يُسمح فقط بملفات PDF");

                }

                // If you want to make sure of the size, for example (3 MB)
                if (file.Length > 3 * 1024 * 1024)
                {
                    isPdf = false;
                    return Resource1.ImageFileMore3mg;
                }

                return "OK";
                #endregion
            }
            else
            {
                return "null";
            }
        }

        public static void DeleteImageFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var fullPath = UploadPathHelper.ResolveRelative(relativePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public static bool IsFileExist(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return false;

            var fullPath = UploadPathHelper.ResolveRelative(relativePath);
            return File.Exists(fullPath);
        }

        public static async Task<string> SaveTempAsync(IFormFile file)
        {
            if (file == null) return null;

            if (!IsExtensionAllowed(file.FileName))
                throw new InvalidOperationException("File extension not allowed.");

            if (!IsFileContentValid(file))
                throw new InvalidOperationException("File content does not match its extension.");

            var ext = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid() + ext;
            var postTempPath = "uploads/temp" + "/" + fileName;
            var tempFolder = UploadPathHelper.Combine("temp");
            Directory.CreateDirectory(tempFolder);
            var tempPath = Path.Combine(tempFolder, fileName);

            using (var stream = new FileStream(tempPath, FileMode.Create))
                await file.CopyToAsync(stream);

            return postTempPath;
        }

        public static string MoveTempToFinal(string tempFileName, out string finalFileName,string folderName)
        {
            finalFileName = null;

            if (string.IsNullOrEmpty(tempFileName)) return "";

            var tempPath = UploadPathHelper.ResolveRelative(tempFileName);
            if (!File.Exists(tempPath)) return "";

            finalFileName = Guid.NewGuid() + Path.GetExtension(tempFileName);
            var newPath = "uploads/"+ folderName +"/"+ finalFileName;
            var uploadsFolder = UploadPathHelper.Combine(folderName);
            Directory.CreateDirectory(uploadsFolder);
            var finalPath = Path.Combine(uploadsFolder, finalFileName);

            File.Move(tempPath, finalPath);
            return newPath;
        }

        public static IFormFile? ConvertToIFormFile(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            var filePath1 = filePath.Replace("wwwroot/", "").Replace("wwwroot", "");
            filePath1 = filePath1.TrimStart('/');
            if (string.IsNullOrEmpty(filePath1)|| string.IsNullOrWhiteSpace(filePath1)) return null;

            var fullPath = UploadPathHelper.ResolveRelative(filePath1);

            var bytes = System.IO.File.ReadAllBytes(fullPath);
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, bytes.Length, "file", Path.GetFileName(fullPath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }

}
