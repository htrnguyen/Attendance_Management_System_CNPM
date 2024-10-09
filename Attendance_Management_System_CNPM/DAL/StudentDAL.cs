using Attendance_Management_System_CNPM.Models;
using AttendanceManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attendance_Management_System_CNPM.DAL
{
    public class StudentDAL
    {
        private readonly string _connectionString;
        public StudentDAL()
        {
            _connectionString = "Data Source=attendance_database.db";
        }
        // Lấy toàn bộ kỳ học của sinh viên
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
                                Enrollments e
                            JOIN
                                Groups g ON e.groupID = g.groupID
                            JOIN
                                Courses c ON g.courseID = c.courseID
                            JOIN
                                Terms t ON c.termID = t.termID
                            WHERE
                                e.studentID = @UserID
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
        // Lấy ID kỳ học của sinh viên
        public int GetTermID(string TermName)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                            SELECT termID
                            FROM Terms
                            WHERE termName = @TermName
                            ";
                    cmd.Parameters.AddWithValue("@TermName", TermName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["termID"]);
                        }
                        return -1;
                    }
                }
            }
        }
        // Lấy toàn bộ môn học của sinh viên trong kỳ học
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
                        u_teacher.userID,
                        cl.classID,
                        cl.className 
                    FROM 
                        Enrollments e
                    JOIN 
                        Groups g ON e.groupID = g.groupID
                    JOIN 
                        Courses c ON g.courseID = c.courseID
                    JOIN 
                        Terms t ON c.termID = t.termID
                    JOIN 
                        Users u ON e.studentID = u.userID
                    JOIN 
                        CourseAssignments ca ON c.courseID = ca.courseID
                    JOIN 
                        Users u_teacher ON ca.teacherID = u_teacher.userID
                    JOIN 
                        Classes cl ON g.classID = cl.classID  -- Kết nối với bảng Classes để lấy tên lớp
                    WHERE 
                        u.userID = @UserID
                        AND u.roleID = 1  -- Chỉ lọc sinh viên
                        AND t.termID = @TermID;  -- Điều kiện cho termID
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
                                TeacherID = Convert.ToInt32(reader["userID"]),
                                ClassID = Convert.ToInt32(reader["classID"]),
                                ClassName = reader["className"].ToString()
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
                        Weeks w ON c.courseID = w.courseID  -- Thay đổi từ Sessions sang Weeks
                    JOIN 
                        Terms t ON c.termID = t.termID
                    WHERE 
                        c.courseID = @CourseID
                    ORDER BY 
                        w.weekNumber;  -- Sắp xếp theo tuần
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
        // Lấy toàn bộ thông báo của tuần 
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
        // Kiểm tra Giáo viên đã tạo link điểm danh chưa
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
        // Lấy toạ độ của lớp
        public List<string> GetClassCoordinates(int ClassID)
        {
            List<string> coordinates = new List<string>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                    SELECT 
                        latitude,
                        longitude
                    FROM 
                        Classes
                    WHERE 
                        classID = @ClassID
                    ";
                    cmd.Parameters.AddWithValue("@ClassID", ClassID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            coordinates.Add(reader["latitude"].ToString());
                            coordinates.Add(reader["longitude"].ToString());
                        }
                    }
                }
            }
            return coordinates;
        }
    }
}
