using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AttendanceManagementSystem.DAL
{
    public class GoogleSheetsRepository
    {
        private readonly SheetsService _sheetsService;
        private readonly string _spreadsheetId = "1bTyUFW5CplUfh_TW1dg6HEiZzwgQJgnzLMk8eUmk4-k";
        //private readonly string _spreadsheetId = "SpreeedID";
        private readonly string jsonPath;

        public GoogleSheetsRepository()
        {
            // Lấy thông tin Google API JSON từ AppSettings
            string googleApiJson = ConfigurationManager.AppSettings["GoogleApiJson"];

            // Lưu JSON vào file tạm thời
            jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "temp_google_credentials.json");
            File.WriteAllText(jsonPath, googleApiJson);

            // Khởi tạo credential từ file JSON
            GoogleCredential credential;
            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(SheetsService.Scope.Spreadsheets);
            }

            // Khởi tạo SheetsService
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "QuanLyDiemDanh"
            });
        }

        // Phương thức lấy dữ liệu từ một phạm vi của bảng tính
        public async Task<IList<IList<object>>> GetSheetData(string sheetRange)
        {
            try
            {
                var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, sheetRange);
                var response = await request.ExecuteAsync();
                return response.Values;
            }
            catch
            {
                // Nếu có bất kỳ lỗi nào xảy ra, chạy lại lệnh
                return await GetSheetData(sheetRange);
            }
        }

        // Phương thức để cập nhật hàng loạt mật khẩu lên Google Sheets
        public async Task UpdateMultiplePasswordsToGoogleSheets(string sheetRange, List<IList<object>> passwords)
        {
            var valueRange = new ValueRange
            {
                Values = passwords
            };

            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _spreadsheetId, sheetRange);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            await updateRequest.ExecuteAsync();
        }
        // Phương thức cập nhật lên googlesheet
        public async Task UpdateToGoogleSheets(string sheetRange, List<IList<object>> values)
        {
            var valueRange = new ValueRange
            {
                Values = values
            };

            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _spreadsheetId, sheetRange);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            await updateRequest.ExecuteAsync();
        }

        // Xóa file tạm sau khi sử dụng
        public void CleanUp()
        {
            if (File.Exists(jsonPath))
            {
                File.Delete(jsonPath);
            }
        }
    }
}
