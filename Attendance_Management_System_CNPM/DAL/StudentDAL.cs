using Attendance_Management_System_CNPM.Models;
using AttendanceManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        g.classroom,
                        u_teacher.userID 
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
                    WHERE 
                        u.userID = @UserID
                        AND u.roleID = 1
                        AND t.termID = @TermID
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
                                ClassRoom = reader["classroom"].ToString(),
                                TeacherID = Convert.ToInt32(reader["userID"])
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
	                        s.sessionID,
                            s.weekNumber,
                            s.sessionDate
                        FROM 
                            Courses c
                        JOIN 
                            Terms t ON c.termID = t.termID
                        JOIN 
                            Sessions s ON c.courseID = s.courseID
                        WHERE 
                            c.courseID = @CourseID
                        ORDER BY 
                            s.weekNumber, s.sessionDate;
                    ";
                    cmd.Parameters.AddWithValue("@CourseID", CourseID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var week = new Weeks
                            {
                                WeekID = Convert.ToInt32(reader["sessionID"]),
                                WeekNumber = Convert.ToInt32(reader["weekNumber"]),
                                StartDate = reader["sessionDate"].ToString()
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
                            Sessions s ON a.sessionID = s.sessionID
                        WHERE 
                            s.sessionID = @WeekID
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
        public bool CheckAttendanceLink(int sessionID, int courseID, int teacherID)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                        SELECT 
                            al.linkID,
                            al.sessionID,
                            al.teacherID,
                            s.courseID,
                            c.courseCode,
                            s.weekNumber,
                            s.sessionDate,
                            u.fullname AS teacherName
                        FROM 
                            AttendanceLinks al
                        JOIN 
                            Sessions s ON al.sessionID = s.sessionID
                        JOIN 
                            Courses c ON s.courseID = c.courseID
                        JOIN 
                            Users u ON al.teacherID = u.userID
                        WHERE 
                            s.sessionID = @SessionID
                            AND c.courseID = @CourseID
                            AND al.teacherID = @TeacherID;
                    ";
                    cmd.Parameters.AddWithValue("@SessionID", sessionID);
                    cmd.Parameters.AddWithValue("@CourseID", courseID);
                    cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
        }
    }
}
