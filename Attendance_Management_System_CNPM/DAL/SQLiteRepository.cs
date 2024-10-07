using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AttendanceManagementSystem.DAL
{
    public class SQLiteRepository
    {
        private readonly string _connectionString;

        public SQLiteRepository()
        {
            _connectionString = $"Data Source=attendance_database.db;Version=3;";
        }
        // Tạo bảng users trong SQLite
         public void CreateDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Tạo bảng Roles
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Roles (
                    RoleID INTEGER PRIMARY KEY,
                    RoleName TEXT NOT NULL
                )";
                command.ExecuteNonQuery();

                // Tạo bảng Users
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE CHECK (Email LIKE '%_@__%.__%'),
                    FullName TEXT NOT NULL,
                    RoleID INTEGER,
                    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
                )";
                command.ExecuteNonQuery();

                // Tạo bảng Terms
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Terms (
                    TermID INTEGER PRIMARY KEY AUTOINCREMENT,
                    TermName TEXT NOT NULL,
                    StartDate DATE NOT NULL,
                    EndDate DATE NOT NULL
                )";
                command.ExecuteNonQuery();

                // Tạo bảng Courses
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Courses (
                    CourseID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CourseName TEXT NOT NULL,
                    CourseCode TEXT NOT NULL,
                    TermID INTEGER,
                    FOREIGN KEY (TermID) REFERENCES Terms(TermID) ON DELETE CASCADE
                )";
                command.ExecuteNonQuery();

                // Tạo bảng CourseAssignments
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS CourseAssignments (
                    AssignmentID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CourseID INTEGER,
                    TeacherID INTEGER,
                    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE,
                    FOREIGN KEY (TeacherID) REFERENCES Users(UserID) ON DELETE CASCADE
                )";
                command.ExecuteNonQuery();

                // Tạo bảng Groups
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Groups (
                    GroupID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CourseID INTEGER,
                    GroupName TEXT NOT NULL,
                    SessionTime TEXT NOT NULL,
                    ClassRoom TEXT NOT NULL,
                    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE
                )";
                command.ExecuteNonQuery();

                // Tạo bảng Enrollments
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Enrollments (
                    EnrollmentID INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentID INTEGER,
                    GroupID INTEGER,
                    FOREIGN KEY (StudentID) REFERENCES Users(UserID) ON DELETE CASCADE,
                    FOREIGN KEY (GroupID) REFERENCES Groups(GroupID) ON DELETE CASCADE
                )";
                command.ExecuteNonQuery();

                // Tạo bảng Sessions
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Sessions (
                    SessionID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CourseID INTEGER,
                    WeekNumber INTEGER NOT NULL,
                    SessionDate DATE NOT NULL,
                    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE
                )";
                command.ExecuteNonQuery();

                // Tạo bảng Announcements
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Announcements (
                    AnnouncementID INTEGER PRIMARY KEY AUTOINCREMENT,
                    SessionID INTEGER,
                    Content TEXT NOT NULL,
                    FOREIGN KEY (SessionID) REFERENCES Sessions(SessionID) ON DELETE CASCADE
                )";
                command.ExecuteNonQuery();

                // Tạo bảng AttendanceLinks
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS AttendanceLinks (
                    LinkID INTEGER PRIMARY KEY AUTOINCREMENT,
                    SessionID INTEGER,
                    TeacherID INTEGER,
                    Latitude REAL NOT NULL,
                    Longitude REAL NOT NULL,
                    FOREIGN KEY (SessionID) REFERENCES Sessions(SessionID) ON DELETE CASCADE,
                    FOREIGN KEY (TeacherID) REFERENCES Users(UserID) ON DELETE CASCADE
                )";
                command.ExecuteNonQuery();

                // Tạo bảng Attendances
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Attendances (
                    AttendanceID INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentID INTEGER,
                    SessionID INTEGER,
                    Status TEXT NOT NULL CHECK (Status IN ('Có mặt', 'Vắng mặt')),
                    CheckedInAt DATETIME,
                    Latitude REAL,
                    Longitude REAL,
                    FOREIGN KEY (StudentID) REFERENCES Users(UserID) ON DELETE CASCADE,
                    FOREIGN KEY (SessionID) REFERENCES Sessions(SessionID) ON DELETE CASCADE
                )";
                command.ExecuteNonQuery();
            }
        }
        // Xoá dữ liệu cũ trong toàn bộ bảng
        // Xoá dữ liệu cũ trong toàn bộ bảng
        public void ClearDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Xóa dữ liệu từ các bảng theo thứ tự để tránh lỗi khóa ngoại
                command.CommandText = "DELETE FROM Attendances";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM AttendanceLinks";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Announcements";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Sessions";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Enrollments";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Groups";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM CourseAssignments";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Courses";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Terms";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Users";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Roles";
                command.ExecuteNonQuery();

                // Đặt lại autoincrement về 1
                command.CommandText = "DELETE FROM sqlite_sequence";
                command.ExecuteNonQuery();
            }
        }
        // Chèn dữ liệu vào bảng Roles
        public async Task InsertRoleData(IList<IList<object>> roleData)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    for (int i = 1; i < roleData.Count; i++)
                    {
                        var row = roleData[i];
                        command.CommandText = @"
                            INSERT INTO Roles (RoleID, RoleName) 
                            VALUES (@RoleID, @RoleName);
                        ";
                        command.Parameters.AddWithValue("@RoleID", row[0]);
                        command.Parameters.AddWithValue("@RoleName", row[1]);

                        await command.ExecuteNonQueryAsync();
                        command.Parameters.Clear();
                    }

                    transaction.Commit();
                }
            }
        }
        // Chèn dữ liệu vào bảng Users
        public async Task InsertUserData(IList<IList<object>> userData)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    for (int i = 1; i < userData.Count; i++)
                    {
                        var row = userData[i];
                        command.CommandText = @"
                            INSERT INTO Users (Username, Password, Email, FullName, RoleID) 
                            VALUES (@Username, @Password, @Email, @FullName, @RoleID);
                        ";
                        command.Parameters.AddWithValue("@Username", row[1]);
                        command.Parameters.AddWithValue("@Password", row[2]);
                        command.Parameters.AddWithValue("@Email", row[3]);
                        command.Parameters.AddWithValue("@FullName", row[4]);
                        command.Parameters.AddWithValue("@RoleID", row[5]);

                        await command.ExecuteNonQueryAsync();
                        command.Parameters.Clear();
                    }

                    transaction.Commit();
                }
            }
        }
        // Chèn dữ liệu vào bảng Terms
        public async Task InsertTermData(IList<IList<object>> termData)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    for (int i = 1; i < termData.Count; i++)
                    {
                        var row = termData[i];
                        command.CommandText = @"
                            INSERT INTO Terms (TermName, StartDate, EndDate) 
                            VALUES (@TermName, @StartDate, @EndDate);
                        ";
                        command.Parameters.AddWithValue("@TermName", row[1]);
                        command.Parameters.AddWithValue("@StartDate", row[2]);
                        command.Parameters.AddWithValue("@EndDate", row[3]);

                        await command.ExecuteNonQueryAsync();
                        command.Parameters.Clear();
                    }

                    transaction.Commit();
                }
            }
        }
        // Chèn dữ liệu vào bảng Courses
        public async Task InsertCourseData(IList<IList<object>> courseData)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    for (int i = 1; i < courseData.Count; i++)
                    {
                        var row = courseData[i];
                        command.CommandText = @"
                            INSERT INTO Courses (CourseName, CourseCode, TermID)
                            VALUES (@CourseName, @CourseCode, @TermID);
                        ";
                        command.Parameters.AddWithValue("@CourseName", row[1]);
                        command.Parameters.AddWithValue("@CourseCode", row[2]);
                        command.Parameters.AddWithValue("@TermID", row[3]);

                        await command.ExecuteNonQueryAsync();
                        command.Parameters.Clear();
                    }

                    transaction.Commit();
                }
            }
        }
        // Chèn dữ liệu vào bảng CourseAssignments
        public async Task InsertCourseAssignmentData(IList<IList<object>> courseAssignmentData)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    for (int i = 1; i < courseAssignmentData.Count; i++)
                    {
                        var row = courseAssignmentData[i];
                        command.CommandText = @"
                            INSERT INTO CourseAssignments (CourseID, TeacherID) 
                            VALUES (@CourseID, @TeacherID);
                        ";
                        command.Parameters.AddWithValue("@CourseID", row[1]);
                        command.Parameters.AddWithValue("@TeacherID", row[2]);

                        await command.ExecuteNonQueryAsync();
                        command.Parameters.Clear();
                    }

                    transaction.Commit();
                }
            }
        }
        // Chèn dữ liệu vào bảng Groups
        public async Task InsertGroupData(IList<IList<object>> groupData)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    for (int i = 1; i < groupData.Count; i++)
                    {
                        var row = groupData[i];
                        command.CommandText = @"
                            INSERT INTO Groups (CourseID, GroupName, SessionTime, ClassRoom)
                            VALUES (@CourseID, @GroupName, @SessionTime, @ClassRoom);
                        ";
                        command.Parameters.AddWithValue("@CourseID", row[1]);
                        command.Parameters.AddWithValue("@GroupName", row[2]);
                        command.Parameters.AddWithValue("@SessionTime", row[3]);
                        command.Parameters.AddWithValue("@ClassRoom", row[4]);

                        await command.ExecuteNonQueryAsync();
                        command.Parameters.Clear();
                    }

                    transaction.Commit();
                }
            }
        }
        // Chèn dữ liệu vào bảng Enrollments
        public async Task InsertEnrollmentData(IList<IList<object>> enrollmentData)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    for (int i = 1; i < enrollmentData.Count; i++)
                    {
                        var row = enrollmentData[i];
                        command.CommandText = @"
                            INSERT INTO Enrollments (StudentID, GroupID) 
                            VALUES (@StudentID, @GroupID);
                        ";
                        command.Parameters.AddWithValue("@StudentID", row[1]);
                        command.Parameters.AddWithValue("@GroupID", row[2]);

                        await command.ExecuteNonQueryAsync();
                        command.Parameters.Clear();
                    }

                    transaction.Commit();
                }
            }
        }
        // Chèn dữ liệu vào bảng Sessions
        public async Task InsertSessionData(IList<IList<object>> sessionData)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    for (int i = 1; i < sessionData.Count; i++)
                    {
                        var row = sessionData[i];
                        command.CommandText = @"
                            INSERT INTO Sessions (CourseID, WeekNumber, SessionDate) 
                            VALUES (@CourseID, @WeekNumber, @SessionDate);
                        ";
                        command.Parameters.AddWithValue("@CourseID", row[1]);
                        command.Parameters.AddWithValue("@WeekNumber", row[2]);
                        command.Parameters.AddWithValue("@SessionDate", row[3]);

                        await command.ExecuteNonQueryAsync();
                        command.Parameters.Clear();
                    }

                    transaction.Commit();
                }
            }
        }
        // Chèn dữ liệu vào bảng Announcements
        public async Task InsertAnnouncementData(IList<IList<object>> announcementData)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    for (int i = 1; i < announcementData.Count; i++)
                    {
                        var row = announcementData[i];
                        command.CommandText = @"
                            INSERT INTO Announcements (SessionID, Content) 
                            VALUES (@SessionID, @Content);
                        ";
                        command.Parameters.AddWithValue("@SessionID", row[1]);
                        command.Parameters.AddWithValue("@Content", row[2]);

                        await command.ExecuteNonQueryAsync();
                        command.Parameters.Clear();
                    }

                    transaction.Commit();
                }
            }
        }
        // Chèn dữ liệu vào bảng AttendanceLinks
        public async Task InsertAttendanceLinkData(IList<IList<object>> attendanceLinkData)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    for (int i = 1; i < attendanceLinkData.Count; i++)
                    {
                        var row = attendanceLinkData[i];
                        command.CommandText = @"
                            INSERT INTO AttendanceLinks (SessionID, TeacherID, Latitude, Longitude) 
                            VALUES (@SessionID, @TeacherID, @Latitude, @Longitude);
                        ";
                        command.Parameters.AddWithValue("@SessionID", row[1]);
                        command.Parameters.AddWithValue("@TeacherID", row[2]);
                        command.Parameters.AddWithValue("@Latitude", row[3]);
                        command.Parameters.AddWithValue("@Longitude", row[4]);

                        await command.ExecuteNonQueryAsync();
                        command.Parameters.Clear();
                    }

                    transaction.Commit();
                }
            }
        }
        // Chèn dữ liệu vào bảng Attendances
        public async Task InsertAttendanceData(IList<IList<object>> attendanceData)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    for (int i = 1; i < attendanceData.Count; i++)
                    {
                        var row = attendanceData[i];
                        command.CommandText = @"
                            INSERT INTO Attendances (StudentID, SessionID, Status, CheckedInAt, Latitude, Longitude) 
                            VALUES (@StudentID, @SessionID, @Status, @CheckedInAt, @Latitude, @Longitude);
                        ";
                        command.Parameters.AddWithValue("@StudentID", row[1]);
                        command.Parameters.AddWithValue("@SessionID", row[2]);
                        command.Parameters.AddWithValue("@Status", row[3]);
                        command.Parameters.AddWithValue("@CheckedInAt", row[4]);
                        command.Parameters.AddWithValue("@Latitude", row[5]);
                        command.Parameters.AddWithValue("@Longitude", row[6]);

                        await command.ExecuteNonQueryAsync();
                        command.Parameters.Clear();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}

