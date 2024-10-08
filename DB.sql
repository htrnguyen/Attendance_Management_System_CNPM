-- Tạo cơ sở dữ liệu
CREATE DATABASE Attendance_Management_System_CNPM;
GO

-- Sử dụng cơ sở dữ liệu
USE Attendance_Management_System_CNPM;
GO

-- Tạo bảng Roles
CREATE TABLE Roles (
    roleID TINYINT PRIMARY KEY,
    roleName NVARCHAR(50) NOT NULL
);
GO

-- Chèn dữ liệu vào bảng Roles
INSERT INTO Roles (roleID, roleName) VALUES
(1, N'Sinh viên'),
(2, N'Giáo viên'),
(3, N'Admin');
GO

-- Tạo bảng Users
CREATE TABLE Users (
    userID INT PRIMARY KEY IDENTITY(1,1),
    username NVARCHAR(50) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE CHECK (email LIKE '%_@__%.__%'),
    fullname NVARCHAR(100) NOT NULL,
    roleID TINYINT,
    FOREIGN KEY (roleID) REFERENCES Roles(roleID)
);
GO

-- Tạo bảng Terms
CREATE TABLE Terms (
    termID INT PRIMARY KEY IDENTITY(1,1),
    termName NVARCHAR(100) NOT NULL,
    startDate DATE NOT NULL,
    endDate DATE NOT NULL
);
GO

-- Tạo bảng Courses
CREATE TABLE Courses (
    courseID INT PRIMARY KEY IDENTITY(1,1),
    courseName NVARCHAR(100) NOT NULL,
    courseCode NVARCHAR(20) NOT NULL,
    termID INT,
    FOREIGN KEY (termID) REFERENCES Terms(termID) ON DELETE CASCADE
);
GO

-- Tạo bảng CourseAssignments
CREATE TABLE CourseAssignments (
    assignmentID INT PRIMARY KEY IDENTITY(1,1),
    courseID INT,
    teacherID INT,
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE,
    FOREIGN KEY (teacherID) REFERENCES Users(userID) ON DELETE CASCADE
);
GO

-- Tạo bảng Classes
CREATE TABLE Classes (
    classID INT PRIMARY KEY IDENTITY(1,1),
    classCode NVARCHAR(20) NOT NULL UNIQUE,
    className NVARCHAR(100) NOT NULL
);
GO
select * from Classes

-- Tạo bảng Groups
CREATE TABLE Groups (
    groupID INT PRIMARY KEY IDENTITY(1,1),
    courseID INT,
    classID INT,
    groupName NVARCHAR(100) NOT NULL,
    sessionTime NVARCHAR(20) NOT NULL,
    classroom NVARCHAR(50) NOT NULL,
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE,
    FOREIGN KEY (classID) REFERENCES Classes(classID)
);
GO
select * from Groups

-- Tạo bảng Enrollments
CREATE TABLE Enrollments (
    enrollmentID INT PRIMARY KEY IDENTITY(1,1),
    studentID INT,
    groupID INT,
    FOREIGN KEY (studentID) REFERENCES Users(userID) ON DELETE CASCADE,
    FOREIGN KEY (groupID) REFERENCES Groups(groupID) ON DELETE CASCADE
);
GO

-- Tạo bảng Weeks
CREATE TABLE Weeks (
    weekID INT PRIMARY KEY IDENTITY(1,1),
    courseID INT,
    weekNumber INT NOT NULL,
    startDate DATE NOT NULL,
    endDate DATE NOT NULL,
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE
);
GO


select * from 

-- Tạo bảng Announcements
CREATE TABLE Announcements (
    announcementID INT PRIMARY KEY IDENTITY(1,1),
    weekID INT,
    content NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (weekID) REFERENCES Weeks(weekID) ON DELETE CASCADE
);
GO

-- Tạo bảng AttendanceLinks
CREATE TABLE AttendanceLinks (
    linkID INT PRIMARY KEY IDENTITY(1,1),
    weekID INT,
    teacherID INT,
    latitude FLOAT NOT NULL,
    longitude FLOAT NOT NULL,
    FOREIGN KEY (weekID) REFERENCES Weeks(weekID) ON DELETE CASCADE,
    FOREIGN KEY (teacherID) REFERENCES Users(userID) ON DELETE CASCADE
);
GO

-- Tạo bảng Attendances
CREATE TABLE Attendances (
    attendanceID INT PRIMARY KEY IDENTITY(1,1),
    studentID INT,
    weekID INT,
    status NVARCHAR(20) NOT NULL CHECK (status IN ('Có mặt', 'Vắng mặt')),
    checkedInAt DATETIME,
    latitude FLOAT,
    longitude FLOAT,
    FOREIGN KEY (studentID) REFERENCES Users(userID) ON DELETE CASCADE,
    FOREIGN KEY (weekID) REFERENCES Weeks(weekID) ON DELETE CASCADE
);
GO

-- Dữ liệu mẫu cho bảng Users
INSERT INTO Users (username, password, email, fullname, roleID) VALUES
('sinhvien1', 'password123', 'sinhvien1@example.com', 'Nguyễn Văn A', 1),
('sinhvien2', 'password456', 'sinhvien2@example.com', 'Trần Thị B', 1),
('sinhvien3', 'password789', 'sinhvien3@example.com', 'Lê Văn C', 1),
('sinhvien4', 'passwordabc', 'sinhvien4@example.com', 'Phạm Thị D', 1),
('sinhvien5', 'passwordxyz', 'sinhvien5@example.com', 'Hồ Văn E', 1),
('sinhvien6', 'password111', 'sinhvien6@example.com', 'Võ Thị F', 1),
('sinhvien7', 'password222', 'sinhvien7@example.com', 'Đặng Văn G', 1),
('sinhvien8', 'password333', 'sinhvien8@example.com', 'Bùi Thị H', 1),
('sinhvien9', 'password444', 'sinhvien9@example.com', 'Trần Văn I', 1),
('sinhvien10', 'password555', 'sinhvien10@example.com', 'Nguyễn Thị J', 1),
('giaovien1', 'password789', 'giaovien1@example.com', 'Lê Văn C', 2),
('giaovien2', 'passwordabc', 'giaovien2@example.com', 'Phạm Thị D', 2),
('giaovien3', 'passwordxyz', 'giaovien3@example.com', 'Hồ Văn E', 2),
('giaovien4', 'password111', 'giaovien4@example.com', 'Võ Thị F', 2),
('admin1', 'passwordxyz', 'admin1@example.com', 'Hồ Văn E', 3);
GO

-- Dữ liệu mẫu cho bảng Terms
INSERT INTO Terms (termName, startDate, endDate) VALUES
('Học kỳ 1 năm 2023', '2023-09-01', '2024-01-15'),
('Học kỳ 2 năm 2023', '2024-02-01', '2024-06-30'),
('Học kỳ mùa hè 2023', '2023-06-15', '2023-08-31');
GO

-- Dữ liệu mẫu cho bảng Courses
INSERT INTO Courses (courseName, courseCode, termID) VALUES
('Toán cao cấp', 'MATH101', 1),
('Vật lý đại cương', 'PHYS101', 1),
('Lập trình Java', 'JAVA101', 2),
('Cơ sở dữ liệu', 'DB101', 2),
('Giải tích', 'CALC101', 1),
('Hóa học', 'CHEM101', 1),
('Xác suất thống kê', 'STAT101', 1),
('Lập trình C++', 'CPP101', 2),
('Khoa học máy tính', 'CS101', 2),
('Kỹ thuật phần mềm', 'SE101', 3),
('Mạng máy tính', 'NET101', 3),
('An toàn thông tin', 'SEC101', 3);
GO

-- Dữ liệu mẫu cho bảng CourseAssignments
INSERT INTO CourseAssignments (courseID, teacherID) VALUES
(1, 11),
(2, 12),
(3, 11),
(4, 12),
(5, 13),
(6, 14),
(7, 13),
(8, 14),
(9, 11),
(10, 12),
(11, 13),
(12, 14);
GO

-- Dữ liệu mẫu cho bảng Classes
INSERT INTO Classes (classCode, className) VALUES
('CLS101', 'Lớp Toán Cao Cấp'),
('CLS102', 'Lớp Vật Lý Đại Cương'),
('CLS103', 'Lớp Lập Trình Java'),
('CLS104', 'Lớp Cơ Sở Dữ Liệu'),
('CLS105', 'Lớp Giải Tích'),
('CLS106', 'Lớp Hóa Học'),
('CLS107', 'Lớp Xác Suất Thống Kê'),
('CLS108', 'Lớp Lập Trình C++'),
('CLS109', 'Lớp Khoa Học Máy Tính'),
('CLS110', 'Lớp Kỹ Thuật Phần Mềm'),
('CLS111', 'Lớp Mạng Máy Tính'),
('CLS112', 'Lớp An Toàn Thông Tin');
GO

-- Dữ liệu mẫu cho bảng Groups
INSERT INTO Groups (courseID, classID, groupName, sessionTime, classroom) VALUES
(1, 1, 'Nhóm 1', '08:00-09:30', 'Phòng 101'),
(1, 1, 'Nhóm 2', '10:00-11:30', 'Phòng 102'),
(2, 2, 'Nhóm 1', '13:00-14:30', 'Phòng 201'),
(3, 3, 'Nhóm A', '15:00-16:30', 'Phòng 202'),
(3, 3, 'Nhóm B', '08:00-09:30', 'Phòng 301'),
(4, 4, 'Nhóm Alpha', '10:00-11:30', 'Phòng 302'),
(5, 5, 'Nhóm 1', '13:00-14:30', 'Phòng 401'),
(5, 5, 'Nhóm 2', '15:00-16:30', 'Phòng 402'),
(6, 6, 'Nhóm 1', '08:00-09:30', 'Phòng 501'),
(7, 7, 'Nhóm A', '10:00-11:30', 'Phòng 502'),
(7, 7, 'Nhóm B', '13:00-14:30', 'Phòng 601'),
(8, 8, 'Nhóm Alpha', '15:00-16:30', 'Phòng 602'),
(9, 9, 'Nhóm 1', '08:00-09:30', 'Phòng 701'),
(9, 9, 'Nhóm 2', '10:00-11:30', 'Phòng 702'),
(10, 10, 'Nhóm 1', '13:00-14:30', 'Phòng 801'),
(11, 11, 'Nhóm A', '15:00-16:30', 'Phòng 802'),
(11, 11, 'Nhóm B', '08:00-09:30', 'Phòng 901'),
(12, 12, 'Nhóm Alpha', '10:00-11:30', 'Phòng 902');
GO

-- Dữ liệu mẫu cho bảng Enrollments
INSERT INTO Enrollments (studentID, groupID) VALUES
(1, 1),
(2, 1),
(3, 2),
(4, 3),
(5, 4),
(6, 5),
(7, 6),
(8, 7),
(9, 8),
(10, 9),
(1, 10),
(2, 11),
(3, 12),
(4, 13),
(5, 14),
(6, 15),
(7, 16),
(8, 17),
(9, 18);
GO

-- Dữ liệu mẫu cho bảng Weeks
INSERT INTO Weeks (courseID, weekNumber, startDate, endDate) VALUES
(1, 1, '2023-09-01', '2023-09-07'),
(1, 2, '2023-09-08', '2023-09-14'),
(2, 1, '2023-09-01', '2023-09-07'),
(3, 1, '2024-02-01', '2024-02-07'),
(4, 1, '2024-02-01', '2024-02-07'),
(5, 1, '2023-09-01', '2023-09-07'),
(5, 2, '2023-09-08', '2023-09-14'),
(6, 1, '2023-09-01', '2023-09-07'),
(7, 1, '2024-02-01', '2024-02-07'),
(8, 1, '2024-02-01', '2024-02-07'),
(9, 1, '2023-09-01', '2023-09-07'),
(9, 2, '2023-09-08', '2023-09-14'),
(10, 1, '2023-09-01', '2023-09-07'),
(11, 1, '2024-02-01', '2024-02-07'),
(12, 1, '2024-02-01', '2024-02-07'),
(1, 3, '2023-09-15', '2023-09-21'),
(2, 2, '2023-09-08', '2023-09-14'),
(3, 2, '2024-02-08', '2024-02-14'),
(4, 2, '2024-02-08', '2024-02-14'),
(5, 3, '2023-09-15', '2023-09-21'),
(6, 2, '2023-09-08', '2023-09-14'),
(7, 2, '2024-02-08', '2024-02-14'),
(8, 2, '2024-02-08', '2024-02-14'),
(9, 3, '2023-09-15', '2023-09-21'),
(10, 2, '2023-09-08', '2023-09-14'),
(11, 2, '2024-02-08', '2024-02-14'),
(12, 2, '2024-02-08', '2024-02-14');
GO

-- Dữ liệu mẫu cho bảng Announcements
INSERT INTO Announcements (weekID, content) VALUES
(1, 'Bài tập về nhà tuần 1'),
(2, 'Ôn tập cho bài kiểm tra giữa kỳ'),
(3, 'Chuẩn bị bài thuyết trình'),
(4, 'Bài tập về nhà tuần 1'),
(5, 'Bài tập về nhà tuần 1'),
(6, 'Ôn tập cho bài kiểm tra giữa kỳ'),
(7, 'Chuẩn bị bài thuyết trình'),
(8, 'Bài tập về nhà tuần 1'),
(9, 'Bài tập về nhà tuần 1'),
(10, 'Ôn tập cho bài kiểm tra giữa kỳ'),
(11, 'Chuẩn bị bài thuyết trình'),
(12, 'Bài tập về nhà tuần 1'),
(13, 'Bài tập về nhà tuần 2'),
(14, 'Ôn tập cho bài kiểm tra giữa kỳ'),
(15, 'Chuẩn bị bài thuyết trình'),
(16, 'Bài tập về nhà tuần 2'),
(17, 'Bài tập về nhà tuần 2'),
(18, 'Ôn tập cho bài kiểm tra giữa kỳ'),
(19, 'Chuẩn bị bài thuyết trình'),
(20, 'Bài tập về nhà tuần 2'),
(21, 'Bài tập về nhà tuần 3'),
(22, 'Ôn tập cho bài kiểm tra giữa kỳ'),
(23, 'Chuẩn bị bài thuyết trình'),
(24, 'Bài tập về nhà tuần 3'),
(25, 'Bài tập về nhà tuần 3'),
(26, 'Ôn tập cho bài kiểm tra giữa kỳ'),
(27, 'Chuẩn bị bài thuyết trình');
GO

-- Dữ liệu mẫu cho bảng AttendanceLinks
INSERT INTO AttendanceLinks (weekID, teacherID, latitude, longitude) VALUES
(1, 11, 21.0278, 105.8519), -- Hà Nội
(2, 12, 10.7626, 106.6602), -- Hồ Chí Minh
(3, 11, 21.0278, 105.8519),
(4, 12, 10.7626, 106.6602),
(5, 13, 21.0278, 105.8519),
(6, 14, 10.7626, 106.6602),
(7, 13, 21.0278, 105.8519),
(8, 14, 10.7626, 106.6602),
(9, 11, 21.0278, 105.8519),
(10, 12, 10.7626, 106.6602),
(11, 13, 21.0278, 105.8519),
(12, 14, 10.7626, 106.6602),
(13, 11, 21.0278, 105.8519),
(14, 12, 10.7626, 106.6602),
(15, 13, 21.0278, 105.8519),
(16, 14, 10.7626, 106.6602),
(17, 11, 21.0278, 105.8519),
(18, 12, 10.7626, 106.6602),
(19, 13, 21.0278, 105.8519),
(20, 14, 10.7626, 106.6602),
(21, 11, 21.0278, 105.8519),
(22, 12, 10.7626, 106.6602),
(23, 13, 21.0278, 105.8519),
(24, 14, 10.7626, 106.6602),
(25, 11, 21.0278, 105.8519),
(26, 12, 10.7626, 106.6602),
(27, 13, 21.0278, 105.8519);
GO

-- Dữ liệu mẫu cho bảng Attendances
INSERT INTO Attendances (studentID, weekID, status, checkedInAt, latitude, longitude) VALUES
(1, 1, 'Có mặt', '2023-09-05 08:00', 21.03, 105.85),
(2, 1, 'Vắng mặt', NULL, NULL, NULL),
(3, 2, 'Có mặt', '2023-09-12 08:05', 21.032, 105.851),
(4, 3, 'Có mặt', '2023-09-06 08:30', 10.76, 106.66),
(5, 4, 'Có mặt', '2023-09-05 08:00', 21.03, 105.85),
(6, 5, 'Vắng mặt', NULL, NULL, NULL),
(7, 6, 'Có mặt', '2023-09-12 08:05', 21.032, 105.851),
(8, 7, 'Có mặt', '2023-09-06 08:30', 10.76, 106.66),
(9, 8, 'Có mặt', '2023-09-05 08:00', 21.03, 105.85),
(10, 9, 'Vắng mặt', NULL, NULL, NULL),
(1, 10, 'Có mặt', '2023-09-12 08:05', 21.032, 105.851),
(2, 11, 'Có mặt', '2023-09-06 08:30', 10.76, 106.66),
(3, 12, 'Có mặt', '2023-09-05 08:00', 21.03, 105.85),
(4, 13, 'Vắng mặt', NULL, NULL, NULL),
(5, 14, 'Có mặt', '2023-09-12 08:05', 21.032, 105.851),
(6, 15, 'Có mặt', '2023-09-06 08:30', 10.76, 106.66),
(7, 16, 'Có mặt', '2023-09-05 08:00', 21.03, 105.85),
(8, 17, 'Vắng mặt', NULL, NULL, NULL),
(9, 18, 'Có mặt', '2023-09-12 08:05', 21.032, 105.851),
(10, 19, 'Có mặt', '2023-09-06 08:30', 10.76, 106.66);
GO	