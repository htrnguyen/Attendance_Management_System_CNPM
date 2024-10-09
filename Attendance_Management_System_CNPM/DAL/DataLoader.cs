using AttendanceManagementSystem.DAL;
using System;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class DataLoader
{
    private readonly GoogleSheetsRepository _googleSheetsRepo;
    private readonly SQLiteRepository _sqliteRepo;
    private readonly string _connectionString = "Data Source=attendance_database.db;Version=3;";

    public DataLoader()
    {
        _googleSheetsRepo = new GoogleSheetsRepository();
        _sqliteRepo = new SQLiteRepository();
    }

    public async Task LoadDataAsync()
    {
        // Lấy dữ liệu từ Google Sheets
        var rolesData = await _googleSheetsRepo.GetSheetData("Roles!A1:B"); 
        var usersData = await _googleSheetsRepo.GetSheetData("Users!A1:F");
        var termsData = await _googleSheetsRepo.GetSheetData("Terms!A1:D");
        var coursesData = await _googleSheetsRepo.GetSheetData("Courses!A1:D");
        var courseAssignmentsData = await _googleSheetsRepo.GetSheetData("CourseAssignments!A1:C");
        var classesData = await _googleSheetsRepo.GetSheetData("Classes!A1:E");
        var groupsData = await _googleSheetsRepo.GetSheetData("Groups!A1:E");
        var EnrollmentsData = await _googleSheetsRepo.GetSheetData("Enrollments!A1:C");
        var weeksData = await _googleSheetsRepo.GetSheetData("Weeks!A1:F");
        var announcementsData = await _googleSheetsRepo.GetSheetData("Announcements!A1:C");
        var attendancesData = await _googleSheetsRepo.GetSheetData("Attendances!A1:G");

        using (var connection = new SQLiteConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();

            // Tắt synchronous và đặt journal mode thành MEMORY để tối ưu tốc độ ghi
            command.CommandText = "PRAGMA synchronous = OFF;";   
            await command.ExecuteNonQueryAsync();
            command.CommandText = "PRAGMA journal_mode = MEMORY;";  
            await command.ExecuteNonQueryAsync();
            command.CommandText = "PRAGMA foreign_keys = OFF;";  
            await command.ExecuteNonQueryAsync();


            // Tạo cơ sở dữ liệu và xóa dữ liệu cũ
            _sqliteRepo.CreateDatabase();
            _sqliteRepo.ClearDatabase();

            // Chèn dữ liệu vào các bảng tương ứng
            await _sqliteRepo.InsertRoleData(rolesData);
            await _sqliteRepo.InsertUserData(usersData);
            await _sqliteRepo.InsertTermData(termsData);
            await _sqliteRepo.InsertCourseData(coursesData);
            await _sqliteRepo.InsertCourseAssignmentData(courseAssignmentsData);
            await _sqliteRepo.InsertClassData(classesData);
            await _sqliteRepo.InsertGroupData(groupsData);
            await _sqliteRepo.InsertEnrollmentData(EnrollmentsData);
            await _sqliteRepo.InsertWeekData(weeksData);
            await _sqliteRepo.InsertAnnouncementData(announcementsData);
            await _sqliteRepo.InsertAttendanceData(attendancesData);

            // Đặt lại synchronous và journal mode sau khi import để đảm bảo an toàn dữ liệu
            command.CommandText = "PRAGMA synchronous = NORMAL;";  
            await command.ExecuteNonQueryAsync();
            command.CommandText = "PRAGMA journal_mode = WAL;";    
            await command.ExecuteNonQueryAsync();
            command.CommandText = "PRAGMA foreign_keys = ON;";   
            await command.ExecuteNonQueryAsync();
        }

        // Hiển thị thông báo khi dữ liệu đã được tải thành công
        //MessageBox.Show("Dữ liệu đã được tải thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // Load bảng Classes va Weeks
    public async Task LoadClassesAndWeeksAsync()
    {
        var classesData = await _googleSheetsRepo.GetSheetData("Classes!A1:E");
        var weeksData = await _googleSheetsRepo.GetSheetData("Weeks!A1:F");

        using (var connection = new SQLiteConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();

            // Tắt synchronous và đặt journal mode thành MEMORY để tối ưu tốc độ ghi
            command.CommandText = "PRAGMA synchronous = OFF;";
            await command.ExecuteNonQueryAsync();
            command.CommandText = "PRAGMA journal_mode = MEMORY;";
            await command.ExecuteNonQueryAsync();
            command.CommandText = "PRAGMA foreign_keys = OFF;";
            await command.ExecuteNonQueryAsync();

            // Tạo cơ sở dữ liệu và xóa dữ liệu cũ
            _sqliteRepo.ClearClassesAndWeeks();

            // Chèn dữ liệu vào các bảng tương ứng
            await _sqliteRepo.InsertClassData(classesData);
            await _sqliteRepo.InsertWeekData(weeksData);

            // Đặt lại synchronous và journal mode sau khi import để đảm bảo an toàn dữ liệu
            command.CommandText = "PRAGMA synchronous = NORMAL;";
            await command.ExecuteNonQueryAsync();
            command.CommandText = "PRAGMA journal_mode = WAL;";
            await command.ExecuteNonQueryAsync();
            command.CommandText = "PRAGMA foreign_keys = ON;";
            await command.ExecuteNonQueryAsync();
        }

        // Hiển thị thông báo khi dữ liệu đã được tải thành công
        MessageBox.Show("Dữ liệu đã được tải thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    public async Task ExecuteWithRetryAsync(Func<Task> action, int maxRetries = 3)
    {
        int retryCount = 0;
        while (true)
        {
            try
            {
                await action();
                break;
            }
            catch (SQLiteException ex) when (ex.Message.Contains("database is locked") && retryCount < maxRetries)
            {
                retryCount++;
                await Task.Delay(1000); // Wait before retrying
            }
        }
    }


}