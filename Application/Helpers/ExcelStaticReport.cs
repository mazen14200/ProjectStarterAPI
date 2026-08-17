using Application.Exceptions;
using Application.Helpers;
using ClosedXML.Excel;
using Application.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services.Admin
{
    public static class ExcelStaticReport
    {
        private static IWebHostEnvironment? _env;
        private static ILogger? _logger;

        private const string FolderAr = "ArDailyReport";
        private const string FolderEn = "EnDailyReport";
        private const string TempFolder = "TempFolder";
        private const string FolderSource = "Source";

        public static void ConfigureExcel(
            IWebHostEnvironment env,
            ILogger logger)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public static (bool Success, string? FilePath)
            ExcelReportAr_withSave_<T>(
                List<T> listData,
                List<string> listTitles,
                int stop = 0,
                string lang = "ar")
        {
            try
            {
                EnsureConfigured();

                var folder = Path.Combine(
                    _env!.WebRootPath,
                    "ReportExcel");

                var reportFolder = Path.Combine(
                    folder,
                    FolderAr);

                var sourceFolder = Path.Combine(
                    folder,
                    FolderSource);

                Directory.CreateDirectory(reportFolder);

                var originalFilePath = Path.Combine(
                    sourceFolder,
                    "ExcelFormula.xlsx");

                var newFileName =
                    "DailyReportAr" +
                    AppDubaiTime.Now.ToString("d-HHmmss") +
                    ".xlsx";

                var newFilePath = Path.Combine(
                    reportFolder,
                    newFileName);

                if (!File.Exists(originalFilePath))
                {
                    throw new FileNotFoundException(
                        "Excel template file was not found.",
                        originalFilePath);
                }

                var bigList = new List<List<string>>
                {
                    listTitles
                };

                using var workbook = new XLWorkbook(originalFilePath);

                DeleteDefaultWorksheet(workbook);

                var worksheet = GetOrCreateReportWorksheet(workbook);

                worksheet.RightToLeft =
                    string.Equals(
                        lang,
                        "ar",
                        StringComparison.OrdinalIgnoreCase);

                // Insert titles
                var rangeTitles =
                    worksheet.Cell("A2")
                        .InsertData(bigList);

                ApplyTitleStyle(rangeTitles);

                IXLRange rangeData;

                if (stop > 0)
                {
                    var limitedData = CreateLimitedData(
                        listData,
                        stop);

                    rangeData =
                        worksheet.Cell("A3")
                            .InsertData(limitedData);
                }
                else
                {
                    rangeData =
                        worksheet.Cell("A3")
                            .InsertData(listData);
                }

                ApplyDataStyle(rangeData);

                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(newFilePath);

                var relativePath = newFilePath
                    .Split("ReportExcel", 2)[1]
                    .Replace("\\", "/");

                return (
                    true,
                    "/ReportExcel" + relativePath
                );
            }
            catch (ServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(
                    ex,
                    "Error generating and saving Arabic Excel report. " +
                    "Report type: {ReportType}, Language: {Language}, " +
                    "Record count: {RecordCount}, Stop: {Stop}",
                    typeof(T).Name,
                    lang,
                    listData?.Count ?? 0,
                    stop);

                throw new ServiceException(
                    "An error occurred while generating the Excel report.",
                    ex);
            }
        }

        public static (bool Success, byte[]? File)
            ExcelReportArEn_<T>(
                List<T> listData,
                List<string> listTitles,
                int stop = 0,
                string lang = "ar",
                string? st20 = null,
                bool mergeSubHeaders = false)
        {
            try
            {
                EnsureConfigured();

                var folder = Path.Combine(
                    _env!.WebRootPath,
                    "ReportExcel");

                var sourceFolder = Path.Combine(
                    folder,
                    FolderSource);

                Directory.CreateDirectory(
                    Path.Combine(folder, FolderAr));

                var originalFilePath = Path.Combine(
                    sourceFolder,
                    "ExcelFormula.xlsx");

                if (!File.Exists(originalFilePath))
                {
                    throw new FileNotFoundException(
                        "Excel template file was not found.",
                        originalFilePath);
                }

                var bigList = new List<List<string>>
                {
                    listTitles
                };

                using var workbook = new XLWorkbook(originalFilePath);

                DeleteDefaultWorksheet(workbook);

                var worksheet = GetOrCreateReportWorksheet(workbook);

                worksheet.RightToLeft =
                    string.Equals(
                        lang,
                        "ar",
                        StringComparison.OrdinalIgnoreCase);

                var startCellTitles = "A1";
                var startCellData = "A2";

                // Optional main title
                if (!string.IsNullOrWhiteSpace(st20))
                {
                    var mergedRange =
                        worksheet.Range("A1:X1").Merge();

                    mergedRange.Value = st20;

                    if (st20.Length < 60)
                    {
                        mergedRange.Style.Alignment.Horizontal =
                            XLAlignmentHorizontalValues.Center;
                    }

                    mergedRange.Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                    mergedRange.Style.Font.Bold = true;
                    mergedRange.Style.Font.FontSize = 16;

                    mergedRange.Style.Fill.BackgroundColor =
                        XLColor.LightGray;

                    startCellTitles = "A2";
                    startCellData = "A3";
                }

                // Insert titles
                var rangeTitles =
                    worksheet.Cell(startCellTitles)
                        .InsertData(bigList);

                ApplyTitleStyle(rangeTitles);

                IXLRange rangeData;

                // Insert data
                if (stop > 0)
                {
                    var limitedData = CreateLimitedData(
                        listData,
                        stop);

                    rangeData =
                        worksheet.Cell(startCellData)
                            .InsertData(limitedData);
                }
                else
                {
                    rangeData =
                        worksheet.Cell(startCellData)
                            .InsertData(listData);
                }

                // Merge category subheaders
                if (mergeSubHeaders &&
                    typeof(T) == typeof(ExcelDataDTO))
                {
                    MergeCategorySubHeaders(
                        worksheet,
                        listData,
                        startCellData,
                        listTitles.Count);
                }

                ApplyDataStyle(rangeData);

                worksheet.Columns().AdjustToContents();

                // Return Excel as byte[]
                using var stream = new MemoryStream();

                workbook.SaveAs(stream);

                return (
                    true,
                    stream.ToArray()
                );
            }
            catch (ServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(
                    ex,
                    "Error generating Excel report. " +
                    "Report type: {ReportType}, Language: {Language}, " +
                    "Record count: {RecordCount}, Stop: {Stop}, " +
                    "MergeSubHeaders: {MergeSubHeaders}",
                    typeof(T).Name,
                    lang,
                    listData?.Count ?? 0,
                    stop,
                    mergeSubHeaders);

                throw new ServiceException(
                    "An error occurred while generating the Excel report.",
                    ex);
            }
        }

        private static void EnsureConfigured()
        {
            if (_env == null)
            {
                throw new ServiceException(
                    "ExcelStaticReport is not configured. " +
                    "Call ConfigureExcel() first.");
            }

            if (_logger == null)
            {
                throw new ServiceException(
                    "ExcelStaticReport logger is not configured. " +
                    "Call ConfigureExcel() with a logger first.");
            }
        }

        private static void DeleteDefaultWorksheet(
            XLWorkbook workbook)
        {
            if (workbook.Worksheets.Any(
                x => x.Name == "Sheet1"))
            {
                workbook.Worksheets.Delete("Sheet1");
            }
        }

        private static IXLWorksheet GetOrCreateReportWorksheet(
            XLWorkbook workbook)
        {
            return workbook.Worksheets.Any(
                x => x.Name == "FougeraClubReport")
                ? workbook.Worksheets.Worksheet("FougeraClubReport")
                : workbook.AddWorksheet("FougeraClubReport");
        }

        private static List<List<string>> CreateLimitedData<T>(
            List<T> listData,
            int stop)
        {
            var properties = typeof(T)
                .GetProperties()
                .Take(stop)
                .ToArray();

            return listData
                .Select(item =>
                    properties
                        .Select(property =>
                            property.GetValue(item)?.ToString() ?? "")
                        .ToList())
                .ToList();
        }

        private static void ApplyTitleStyle(
            IXLRange range)
        {
            range.Style.Fill.BackgroundColor =
                XLColor.LightBlue;

            range.Style.Font.FontColor =
                XLColor.DarkBlue;

            range.Style.Font.Bold = true;

            range.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            range.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;
        }

        private static void ApplyDataStyle(
            IXLRange range)
        {
            range.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            range.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;
        }

        private static void MergeCategorySubHeaders<T>(
            IXLWorksheet worksheet,
            List<T> listData,
            string startCellData,
            int titleCount)
        {
            var dataList =
                listData.Cast<ExcelDataDTO>().ToList();

            var dataFirstRow =
                worksheet.Cell(startCellData)
                    .Address.RowNumber;

            for (var i = 0; i < dataList.Count; i++)
            {
                var dto = dataList[i];

                // Category header row
                if (dto.t2 != null &&
                    dto.t3 == null &&
                    dto.t4 == null &&
                    dto.t5 == null &&
                    dto.t6 == null)
                {
                    var currentRow =
                        dataFirstRow + i;

                    // Merge columns 2 -> last title column
                    worksheet
                        .Range(
                            currentRow,
                            2,
                            currentRow,
                            titleCount)
                        .Merge();

                    var headerRange =
                        worksheet.Range(
                            currentRow,
                            1,
                            currentRow,
                            titleCount);

                    headerRange.Style.Fill.BackgroundColor =
                        XLColor.LightBlue;

                    headerRange.Style.Font.Bold = true;

                    headerRange.Style.Font.FontSize = 12;

                    headerRange.Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Center;

                    headerRange.Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;
                }
            }
        }
    }
}
