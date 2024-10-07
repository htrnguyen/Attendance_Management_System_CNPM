using AttendanceManagementSystem.DAL;
using Attendance_Management_System_CNPM.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attendance_Management_System_CNPM.DAL
{
    public class UserDAL
    {
        private readonly string _connectionString;

        public UserDAL()
        {
            _connectionString = "Data Source=attendance_database.db"; ;
        }
        // Login
        public bool IsLogin(string username, string password)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT * FROM Users WHERE Username = @username AND Password = @password";
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }
        // Forgot Password
        public bool IsEmailExist(string email)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT * FROM Users WHERE Email = @Email";
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }
        // Update Password
        public void UpdatePassword(string email, string password)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "UPDATE Users SET Password = @Password WHERE Email = @Email";
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        //Đồng bộ mật khẩu lên Google Sheets
        public async Task SyncPasswordToGoogleSheets(GoogleSheetsRepository googleSheetsRepo)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT Password FROM Users;
                ";

                var reader = await command.ExecuteReaderAsync();
                var passwords = new List<string>();
                while (await reader.ReadAsync())
                {
                    passwords.Add(reader.GetString(0));
                }

                // Chuẩn bị danh sách giá trị để cập nhật hàng loạt
                var values = new List<IList<object>>();
                foreach (var password in passwords)
                {
                    values.Add(new List<object> { password });
                }

                // Cập nhật mật khẩu lên Google Sheets với tất cả hàng trong một lần
                var range = $"Users!C2:C{passwords.Count + 1}";
                await googleSheetsRepo.UpdateMultiplePasswordsToGoogleSheets(range, values);
            }
        }
        // Lấy thông tin người dùng
        public async Task<User> GetUserInfo(string username)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SQLiteCommand(conn))
                {
                    User user = new User();

                    cmd.CommandText = @"
                        SELECT
                            Users.UserID, 
                            Users.Email, 
                            Users.FullName, 
                            Roles.RoleName
                        FROM Users
                        JOIN Roles ON Users.RoleID = Roles.RoleID
                        WHERE Users.Username = @username
                    ";
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user.UserID = reader.GetInt32(0);
                            user.Email = reader.GetString(1);
                            user.FullName = reader.GetString(2);
                            user.Role = reader.GetString(3);
                            return user;
                        }
                        return null;
                    }
                }
            }
        }
    }
}
