using Data.Constants;
using Data.Models.SMDModels;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Services.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Extenstions
{
    public static class NPOIExtenstions
    {
        #region Styles
        public static ICellStyle Bold(this IWorkbook wb, int height = 11)
        {
            IFont boldFont = wb.CreateFont();
            boldFont.IsBold = true;
            boldFont.FontHeightInPoints = height;
            ICellStyle cellStyle = wb.CreateCellStyle();
            cellStyle.SetFont(boldFont);
            return cellStyle;
        }
        #endregion

        public static IWorkbook ReadAsWorkbook(this IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;
                IWorkbook workbook = new XSSFWorkbook(stream);
                return workbook;
            }
        }

        public async static Task<IWorkbook> ReadAsWorkbookAsync(this IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                if (file == null)
                    throw new ArgumentException(ErrorMessages.FILE_NOT_FOUND);
                await file.CopyToAsync(stream);
                stream.Position = 0;
                IWorkbook workbook = new XSSFWorkbook(stream);
                return workbook;
            }
        }

        /// <summary>
        /// Try Parse a cell value to Datetime if posible
        /// 
        /// </summary>
        /// <param name="cell">NPOI ICell</param>
        /// <returns>DateTime value or throw exception</returns>
        public static DateTime ParseDateTime(this ICell cell)
        {
            try
            {
                // Xóa khoảng trắng ở đầu và cuối chuỗi
                var cellData = cell.ToString().Trim();
                // 3 dòng tiếp theo xóa những khoảng trắng ở giữa chuỗi với độ dài lớn hơn 2
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                var formatedCellData = regex.Replace(cellData, " ");
                return formatedCellData.TryParseDateTime();

            }
            catch (Exception)
            {
                throw new Exception($"Ngày tháng không hợp lệ.");
            }
        }

        /// <summary>
        /// Check row is valid?
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this IRow row)
        {
            return row == null || row.Cells.All(d => d.IsNullOrEmpty());
        }

        /// <summary>
        /// Check cell is valid?
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this ICell cell)
        {
            return cell == null
                || cell.CellType == CellType.Blank;
            //|| (cell.CellType != CellType.Formula && string.IsNullOrWhiteSpace(cell.ToString()));
        }

        public static async Task<ICollection<ReportAggregateModel>> ReadAsReportAggregateModels(this ISheet sheet)
        {
            List<ReportAggregateModel> models = new List<ReportAggregateModel>();
            try
            {
                var tasks = new List<Task<ReportAggregateModel>>();
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    tasks.Add(sheet.ReadAsReportAggregateModel(i));
                }
                var result = await TaskExtensions.WhenAllOrException(tasks);
                string failLog = "";
                foreach (var item in result)
                {
                    if (!item.IsSuccess)
                    {
                        failLog += item.Exception.Message + Environment.NewLine;
                        continue;
                    }
                    if (item.Result != null)
                        models.Add(item.Result);
                }
                if (!string.IsNullOrEmpty(failLog))
                    throw new Exception(failLog);
            }
            catch (Exception)
            {
                throw;
            }
            return models;
        }

        private static async Task<ReportAggregateModel> ReadAsReportAggregateModel(this ISheet sh, int rowIndex)
        {
            return await Task.Run(() =>
            {
                try
                {
                    IRow row = sh.GetRow(rowIndex);
                    if (row.IsNullOrEmpty())
                        return null;

                    ReportAggregateModel model = new ReportAggregateModel();
                    short cellIndex = row.FirstCellNum;
                    ICell cell;
                    model.Row = rowIndex + 1;

                    // Period
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Period.");
                    model.Period = ReportPeriod.MONTH;
                    // Year
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Year.");
                    model.Year = cell.ParseToYear();
                    // Month
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Month.");
                    model.Month = cell.ParseToMonth();
                    // CBOCode
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing CBO Code.");
                    model.CBOCode = cell.ToString().Trim();
                    // PSNU
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing PSNU.");
                    model.PSNU = LocationUtils.GetProvinceValue(cell.ToString().Trim());
                    if (string.IsNullOrEmpty(model.PSNU))
                        throw new Exception($"Cannot convert PSNU: '{cell.ToString().Trim()}' to a valid PSNU.");
                    // Indicator Code
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Indicator Code.");
                    model.IndicatorCode = cell.ToString().Trim();
                    // Value
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Value.");
                    model.Value = double.Parse(cell.ToString().Trim());

                    return model;
                }
                catch (Exception e)
                {
                    throw new Exception("Row: " + (rowIndex + 1) + ", " + e.Message);
                }
            });
        }

        public static async Task<ICollection<ReportIndividualModel>> ReadAsReportIndividualModels(this ISheet sheet)
        {
            List<ReportIndividualModel> models = new List<ReportIndividualModel>();
            try
            {
                var tasks = new List<Task<ReportIndividualModel>>();
                for (int i = 4; i <= sheet.LastRowNum; i++)
                {
                    tasks.Add(sheet.ReadAsReportIndividualModel(i));
                }
                var result = await TaskExtensions.WhenAllOrException(tasks);
                string failLog = "";
                foreach (var item in result)
                {
                    if (!item.IsSuccess)
                    {
                        failLog += item.Exception.Message + Environment.NewLine;
                        continue;
                    }
                    if (item.Result != null)
                        models.Add(item.Result);
                }
                if (!string.IsNullOrEmpty(failLog))
                    throw new Exception(failLog);
            }
            catch (Exception)
            {
                throw;
            }
            return models;
        }

        private static async Task<ReportIndividualModel> ReadAsReportIndividualModel(this ISheet sh, int rowIndex)
        {
            return await Task.Run(() =>
            {
                IRow row = sh.GetRow(rowIndex);
                if (row.IsNullOrEmpty())
                    return null;
                try
                {
                    ReportIndividualModel model = new ReportIndividualModel();
                    short cellIndex = row.FirstCellNum;
                    ICell cell;
                    model.Row = rowIndex + 1;

                    #region info
                    // No
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing No.");
                    model.No = int.Parse(cell.ToString().Trim());
                    // PSNU
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing PSNU.");
                    model.PSNU = LocationUtils.GetProvinceValue(cell.ToString().Trim());
                    if (string.IsNullOrEmpty(model.PSNU))
                        throw new Exception($"Cannot convert PSNU: '{cell.ToString().Trim()}' to a valid PSNU.");
                    // MoPName
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing MoPName.");
                    model.MoPName = cell.ToString().Trim();
                    // CBOName
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing CBOName.");
                    model.CBOName = cell.ToString().Trim();
                    // SupporterName
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing SupporterName.");
                    model.SupporterName = cell.ToString().Trim();
                    // ReachCode
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing ReachCode.");
                    model.ReachCode = cell.ToString().Trim();
                    #endregion
                    #region HIVTestingService
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        model.HIVTestingService.LayTestingCode = cell.ToString().Trim();
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        model.HIVTestingService.HTCTestCode = cell.ToString().Trim();
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        model.HIVTestingService.HTCSite = cell.ToString().Trim();

                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    int dateOT = 0, monthOT = 0, yearOT = 0;
                    if (!cell.IsNullOrEmpty())
                    {
                        dateOT = cell.ParseToDate();
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        monthOT = cell.ParseToMonth();
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        yearOT = cell.ParseToYear();
                    }
                    if (dateOT != 0 && monthOT != 0 && yearOT != 0)
                    {
                        var DoT = new DateTime(yearOT, monthOT, dateOT);
                        model.HIVTestingService.ReportDate = DoT;
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        model.HIVTestingService.TestResult = cell.ToString().Trim();
                    }
                    #endregion
                    #region ReferralService
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        model.ReferralService.ServiceName = cell.ToString().Trim();
                        if(model.HIVTestingService.TestResult.Equals("Positive", StringComparison.OrdinalIgnoreCase)
                        && !model.ReferralService.ServiceName.Equals("ARV", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new Exception("If Test result is \"Positive\", Referral Service Name must be \"ARV\"");
                        }
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        model.ReferralService.ClientID = cell.ToString().Trim();
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        model.ReferralService.FacilityName = cell.ToString().Trim();
                    }
                    //
                    int dateOR = 0, monthOR = 0, yearOR = 0;
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        dateOR = cell.ParseToDate();
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        monthOR = cell.ParseToMonth();
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        yearOR = cell.ParseToYear();
                    }
                    if (dateOR != 0 && monthOR != 0 && yearOR != 0)
                    {
                        var DoR = new DateTime(yearOR, monthOR, dateOR);
                        model.ReferralService.ReportDate = DoR;
                    };
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        model.ReferralSlip = cell.ToString().Trim();
                    }
                    #endregion
                    #region VerificationResult
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        model.VerificationResult.NewCase = cell.ToString().Trim();
                    }
                    //
                    int dateOV = 0, monthOV = 0, yearOV = 0;
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty() && !string.IsNullOrEmpty(model.VerificationResult.NewCase) && model.VerificationResult.NewCase != "Pending")
                    {
                        dateOV = cell.ParseToDate();
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty() && !string.IsNullOrEmpty(model.VerificationResult.NewCase) && model.VerificationResult.NewCase != "Pending")
                    {
                        monthOV = cell.ParseToMonth();
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty() && !string.IsNullOrEmpty(model.VerificationResult.NewCase) && model.VerificationResult.NewCase != "Pending")
                    {
                        yearOV = cell.ParseToYear();
                    }
                    if (dateOV != 0 && monthOV != 0 && yearOV != 0)
                    {
                        var DoV = new DateTime(yearOV, monthOV, dateOV);
                        model.VerificationResult.ReportDate = DoV;
                    }
                    #endregion
                    #region Reporting Period
                    //
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Month in Reporting Period.");
                    var monthRP = cell.ParseToMonth();
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Year in Reporting Period.");
                    var yearRP = cell.ParseToYear();
                    var rp = new DateTime(yearRP, monthRP, DateTime.DaysInMonth(yearRP, monthRP));
                    model.ReportingPeriod = rp;
                    #endregion
                    #region Update Date
                    //
                    int dateUD = 0, monthUD = 0, yearUD = 0;
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        
                        dateUD = cell.ParseToDate();
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        monthUD = cell.ParseToMonth();
                    }
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (!cell.IsNullOrEmpty())
                    {
                        yearUD = cell.ParseToYear();
                    }
                    if (dateUD != 0 && monthUD != 0 && yearUD != 0)
                    {
                        var dUD = new DateTime(yearUD, monthUD, dateUD);
                        model.UpdatedDate = dUD;
                    }
                    #endregion

                    var validator = new DataAnnotationsValidator.DataAnnotationsValidator();
                    var context = new ValidationContext(model, null, null);
                    var results = new List<ValidationResult>();
                    var isValid = validator.TryValidateObjectRecursive(model, results);
                    if (!isValid)
                    {
                        throw new Exception(String.Join("\n ", results.Select(_ => _.ErrorMessage)));
                    }

                    return model;
                }
                catch (Exception e)
                {
                    throw new Exception("Row: " + (rowIndex + 1) + " | " + e.Message);
                }
            });
        }

        public static async Task<ICollection<ReportPaymentModel>> ReadAsReportPaymentModels(this ISheet sheet)
        {
            List<ReportPaymentModel> models = new List<ReportPaymentModel>();
            try
            {
                var tasks = new List<Task<ReportPaymentModel>>();
                for (int i = 3; i <= sheet.LastRowNum; i++)
                {
                    tasks.Add(sheet.ReadAsReportPaymentModel(i));
                }
                var result = await TaskExtensions.WhenAllOrException(tasks);
                string failLog = "";
                foreach (var item in result)
                {
                    if (!item.IsSuccess)
                    {
                        failLog += item.Exception.Message + Environment.NewLine;
                        continue;
                    }
                    if (item.Result != null)
                        models.Add(item.Result);
                }
                if (!string.IsNullOrEmpty(failLog))
                    throw new Exception(failLog);
            }
            catch (Exception)
            {
                throw;
            }
            return models;
        }

        private static async Task<ReportPaymentModel> ReadAsReportPaymentModel(this ISheet sh, int rowIndex)
        {
            return await Task.Run(() =>
            {
                try
                {
                    IRow row = sh.GetRow(rowIndex);
                    if (row.IsNullOrEmpty())
                        return null;

                    ReportPaymentModel model = new ReportPaymentModel();
                    short cellIndex = row.FirstCellNum;
                    ICell cell;
                    model.Row = rowIndex + 1;

                    #region info
                    // No
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing No.");
                    model.No = int.Parse(cell.ToString().Trim());
                    // PSNU
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing PSNU.");
                    model.PSNU = LocationUtils.GetProvinceValue(cell.ToString().Trim());
                    if (string.IsNullOrEmpty(model.PSNU))
                        throw new Exception($"Cannot convert PSNU: '{cell.ToString().Trim()}' to a valid PSNU.");
                    // MoPName
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing MoPName.");
                    model.MoPName = cell.ToString().Trim();
                    // CBOName
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing CBOName.");
                    model.CBOName = cell.ToString().Trim();
                    #endregion
                    #region Reporting Period
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Month in Payment Period.");
                    var monthRP = cell.ParseToMonth();
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Year in Payment Period.");
                    var yearRP = cell.ParseToYear();
                    var rp = new DateTime(yearRP, monthRP, DateTime.DaysInMonth(yearRP, monthRP));
                    model.ReportingPeriod = rp;
                    #endregion
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Package Number.");
                    model.PackageCode = cell.ToString().Trim();
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Total amount.");
                    model.TotalAmount = double.Parse(cell.ToString().Trim());
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Date in Date of Payment.");
                    var dateOP = cell.ParseToDate();
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Month in Date of Payment.");
                    var monthOP = cell.ParseToMonth();
                    // 
                    cell = row.GetCell(cellIndex++);
                    if (cell.IsNullOrEmpty())
                        throw new Exception("Missing Year in Date of Payment.");
                    var yearOP = cell.ParseToYear();
                    var DoP = new DateTime(yearOP, monthOP, dateOP);
                    model.DateOfPayment = DoP;

                    var validator = new DataAnnotationsValidator.DataAnnotationsValidator();
                    var context = new ValidationContext(model, null, null);
                    var results = new List<ValidationResult>();
                    var isValid = validator.TryValidateObjectRecursive(model, results);
                    if (!isValid)
                    {
                        throw new Exception(String.Join("\n ", results.Select(_ => _.ErrorMessage)));
                    }

                    return model;
                }
                catch (Exception e)
                {
                    throw new Exception("Row: " + (rowIndex + 1) + " | " + e.Message);
                }
            });
        }

        public static int ParseToYear(this ICell cell)
        {
            try
            {
                var year = int.Parse(cell.ToString().Trim());
                if (year < 2000)
                    throw new Exception();
                return year;
            }
            catch (Exception)
            {
                throw new Exception("Invalid year:" + cell.ToString().Trim());
            }
        }

        public static int ParseToMonth(this ICell cell)
        {
            try
            {
                var month = int.Parse(cell.ToString().Trim());
                if (month < 1 || month > 12)
                    throw new Exception();
                return month;
            }
            catch (Exception)
            {
                throw new Exception("Invalid month:" + cell.ToString().Trim());
            }
        }

        public static int ParseToDate(this ICell cell)
        {
            try
            {
                var date = int.Parse(cell.ToString().Trim());
                if (date < 1 || date > 31)
                    throw new Exception();
                return date;
            }
            catch (Exception)
            {
                throw new Exception($"Invalid date at column no {cell.ColumnIndex}:" + cell.ToString().Trim());
            }
        }

        public static int ParseToPackageCode(this ICell cell)
        {
            try
            {
                var no = int.Parse(cell.ToString().Trim());
                if (no < 1 || no > 4)
                    throw new Exception();
                return no;
            }
            catch (Exception)
            {
                throw new Exception("Invalid Package Number:" + cell.ToString().Trim());
            }
        }

        public static void SetRowNoneNullCellStyle(this IRow row, ICellStyle style)
        {
            if (row != null)
            {
                foreach (var cell in row.Cells)
                {
                    cell.CellStyle = style;
                }
            }
        }
    }
}
