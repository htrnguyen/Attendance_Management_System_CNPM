using Attendance_Management_System_CNPM.Models;
using AttendanceManagementSystem.DAL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attendance_Management_System_CNPM.DAL
{
    public class TeacherDAL
    {
        private readonly string _connectionString;

        public TeacherDAL()
        {
            _connectionString = "Data Source=attendance_database.db";
        }
        // Lấy toàn bộ kỳ học của giáo viên
        public List<Terms> GetTerms(int UserID)
        {
            List<Terms> terms = new List<Terms>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                        SELECT DISTINCT
                            t.termID,
                            t.termName,
                            t.startDate,
                            t.endDate
                        FROM
                            CourseAssignments ca
                        JOIN
                            Courses c ON ca.courseID = c.courseID
                        JOIN
                            Terms t ON c.termID = t.termID
                        WHERE
                            ca.teacherID = @UserID;
                            ";
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var term = new Terms
                            {
                                TermID = Convert.ToInt32(reader["termID"]),
                                TermName = reader["termName"].ToString(),
                                StartDate = reader["startDate"].ToString(),
                                EndDate = reader["endDate"].ToString()
                            };
                            terms.Add(term);
                        }
                    }
                }
            }
            return terms;
        }
        // Lấy toàn bộ môn học của giáo viên
        public List<Courses> GetCourses(int UserID, int TermID)
        {
            List<Courses> courses = new List<Courses>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                    SELECT DISTINCT 
                        c.courseID, 
                        c.courseName, 
                        c.courseCode,
                        g.groupName,
                        g.sessionTime,
                        cl.classID,
                        cl.className
                    FROM 
                        CourseAssignments ca
                    JOIN 
                        Courses c ON ca.courseID = c.courseID
                    JOIN 
                        Groups g ON c.courseID = g.courseID
                    JOIN 
                        Terms t ON c.termID = t.termID
                    JOIN 
                        Users u ON ca.teacherID = u.userID
                    JOIN 
                        Classes cl ON g.classID = cl.classID  -- Kết nối với bảng Classes để lấy tên lớp
                    WHERE 
                        u.userID = @UserID
                        AND u.roleID = 2  -- Chỉ lọc giáo viên
                        AND t.termID = @TermID;
                     ";
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@TermID", TermID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var course = new Courses
                            {
                                CourseID = Convert.ToInt32(reader["courseID"]),
                                CourseName = reader["courseName"].ToString(),
                                CourseCode = reader["courseCode"].ToString(),
                                GroupName = reader["groupName"].ToString(),
                                SessionTime = reader["sessionTime"].ToString(),
                                TeacherID = UserID,
                                ClassID = Convert.ToInt32(reader["classID"]),
                                ClassName = reader["className"].ToString(),
                            };
                            courses.Add(course);
                        }
                    }
                }
            }
            return courses;
        }
        // Lấy toàn bộ tuần học của môn học
        public List<Weeks> GetWeeks(int CourseID)
        {
            List<Weeks> weeks = new List<Weeks>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                    SELECT 
                        w.weekID,
	                    w.weekNumber,
	                    w.startDate,
	                    w.endDate
                    FROM 
                        Courses c
                    JOIN 
                        Terms t ON c.termID = t.termID
                    JOIN 
                        Weeks w ON c.courseID = w.courseID
                    WHERE 
                        c.courseID = @CourseID
                    ";
                    cmd.Parameters.AddWithValue("@CourseID", CourseID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var week = new Weeks
                            {
                                WeekID = Convert.ToInt32(reader["weekID"]),
                                WeekNumber = Convert.ToInt32(reader["weekNumber"]),
                                StartDate = reader["startDate"].ToString(),
                                EndDate = reader["endDate"].ToString()
                            };
                            weeks.Add(week);
                        }
                    }
                }
            }
            return weeks;
        }
        // Lấy toàn bộ thông báo của giáo viên
        public List<Announcements> GetAnnouncements(int WeekID)
        {
            List<Announcements> announcements = new List<Announcements>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                    SELECT 
                        a.announcementID,
                        a.content
                    FROM 
                        Announcements a
                    JOIN 
                        Weeks w ON a.weekID = w.weekID  -- Kết nối bảng Announcements với Weeks
                    WHERE 
                        w.weekID = @WeekID;  -- Sử dụng weekID trong điều kiện
                    ";
                    cmd.Parameters.AddWithValue("@WeekID", WeekID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var announcement = new Announcements
                            {
                                AnnouncementID = Convert.ToInt32(reader["announcementID"]),
                                Content = reader["content"].ToString()
                            };
                            announcements.Add(announcement);
                        }
                    }
                }
            }
            return announcements;
        }
        // Kiểm tra giáo viên đã tạo link điểm danh chưa
        public bool CheckAttendanceLink(int WeekID, int CourseID, int TeacherID)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                        SELECT 
                            w.weekID,
                            w.weekNumber,
                            w.startDate,
                            w.endDate,
                            w.isAttendanceLinkCreated
                        FROM 
                            Weeks w
                        JOIN 
                            Courses c ON w.courseID = c.courseID
                        JOIN 
                            CourseAssignments ca ON c.courseID = ca.courseID
                        JOIN 
                            Users u ON ca.teacherID = u.userID
                        WHERE 
                            u.userID = @TeacherID -- ID của giáo viên
                            AND w.weekID = @WeekID -- ID của tuần cần kiểm tra
                            AND c.courseID = @CourseID; -- ID của khóa học cần kiểm tra
                    ";
                    cmd.Parameters.AddWithValue("@WeekID", WeekID);
                    cmd.Parameters.AddWithValue("@CourseID", CourseID);
                    cmd.Parameters.AddWithValue("@TeacherID", TeacherID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToBoolean(reader["isAttendanceLinkCreated"]);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }
        // Cập nhật toạ độ lên Database
        public bool UpdateCoordinates(int ClassID, int CourseID, string Latitude, string Longitude)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                    UPDATE Classes
                    SET 
                        latitude = @Latitude, 
                        longitude = @Longitude
                    WHERE 
                        classID = @ClassID
                        AND classID IN (
                            SELECT classID 
                            FROM Groups g
                            JOIN Courses c ON g.courseID = c.courseID
                            WHERE c.courseID = @CourseID
                        );
                    ";
                    cmd.Parameters.AddWithValue("@ClassID", ClassID);
                    cmd.Parameters.AddWithValue("@CourseID", CourseID);
                    cmd.Parameters.AddWithValue("@Latitude", Latitude);
                    cmd.Parameters.AddWithValue("@Longitude", Longitude);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        // Cập nhật trạng thái link điểm danh
        public bool UpdateAttendanceLinkStatus(int WeekID, int CourseID, int TeacherID)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                    UPDATE Weeks
                    SET isAttendanceLinkCreated = 1
                    WHERE weekID = @WeekID
                      AND courseID = @CourseID
                      AND courseID IN (
                          SELECT ca.courseID
                          FROM CourseAssignments ca
                          WHERE ca.teacherID = @TeacherID
                      );
                    ";
                    cmd.Parameters.AddWithValue("@WeekID", WeekID);
                    cmd.Parameters.AddWithValue("@CourseID", CourseID);
                    cmd.Parameters.AddWithValue("@TeacherID", TeacherID);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        // Đồng bộ bảng Classes lên GoogleSheet
        public async Task SyncClassessDataToGoogleSheet(GoogleSheetsRepository googleSheetsRepo)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT Latitude, Longitude FROM Classes;
                ";

                var reader = await command.ExecuteReaderAsync();
                var latitudes = new List<string>();
                var longitudes = new List<string>();

                while (await reader.ReadAsync())
                {
                    latitudes.Add(reader["Latitude"].ToString());
                    longitudes.Add(reader["Longitude"].ToString());
                }

                // Chuẩn bị danh sách giá trị để cập nhật hàng loạt
                var values_lat = new List<IList<object>>();
                var values_lon = new List<IList<object>>();

                foreach (var latitude in latitudes)
                {
                    values_lat.Add(new List<object> { latitude });
                }

                foreach (var longitude in longitudes)
                {
                    values_lon.Add(new List<object> { longitude });
                }

                var range_lat = $"Classes!D2:D{latitudes.Count + 1}";
                var range_lon = $"Classes!E2:E{longitudes.Count + 1}";

                await googleSheetsRepo.UpdateToGoogleSheets(range_lat, values_lat);
                await googleSheetsRepo.UpdateToGoogleSheets(range_lon, values_lon);

            }
        }
        // Đồng bộ bảng Weeks lên GoogleSheet
        public async Task SyncWeeksDataToGoogleSheet(GoogleSheetsRepository googleSheetsRepo)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT IsAttendanceLinkCreated FROM Weeks;
                ";

                var reader = await command.ExecuteReaderAsync();
                var IsAttendanceLinkCreateds = new List<int>();
                while (await reader.ReadAsync())
                {
                    IsAttendanceLinkCreateds.Add(Convert.ToInt32(reader["IsAttendanceLinkCreated"]));
                }

                // Chuẩn bị danh sách giá trị để cập nhật hàng loạt
                var values = new List<IList<object>>();
                foreach (var isAttendanceLinkCreated in IsAttendanceLinkCreateds)
                {
                    values.Add(new List<object> { isAttendanceLinkCreated });
                }

                // Cập nhật mật khẩu lên Google Sheets với tất cả hàng trong một lần
                var range = $"Weeks!F2:F{IsAttendanceLinkCreateds.Count + 1}";
                await googleSheetsRepo.UpdateMultiplePasswordsToGoogleSheets(range, values);
            }
        }
    }
}
