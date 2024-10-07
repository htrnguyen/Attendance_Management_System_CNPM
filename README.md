# Hướng Dẫn Cài

Hướng dẫn này sẽ giúp bạn từng bước thiết lập môi trường dự án, tạo một Google Sheet, tải dữ liệu từ file Excel lên, chia sẻ bảng tính, và lấy ID của bảng tính.

## Yêu Cầu Trước Khi Bắt Đầu

Trước khi bắt đầu, hãy đảm bảo bạn có:

-   Một tài khoản Google
-   Quyền truy cập vào Google Sheets
-   [Tải file Excel chứa dữ liệu về](./QuanLyDiemDanh.xlsx)
-   Kết nối Internet

## Bước 1: Thiết Lập Môi Trường Dự Án

1. **Clone Repository**

    ```bash
    git clone https://github.com/htrnguyen/Attendance_Management_System_CNPM.git
    cd Attendance_Management_System_CNPM
    ```

2. **Khởi Động Lại Dự Án**

    - Mở dự án trong Visual Studio.
    - Thiết lập `Program.cs` làm dự án khởi động.
    - Nhấn `F5` để chạy ứng dụng.
    - Nếu gặp lỗi, hãy cài đặt gói `System.Data.SQLite` bằng lệnh sau trong Package Manager Console:

    ```powershell
    Install-Package System.Data.SQLite
    ```

## Bước 2: Tạo Google Sheet và Tải Dữ Liệu

1. **Tạo Google Sheet Mới**

    - Truy cập [Google Sheets](https://sheets.google.com).
    - Tạo một bảng tính mới.

2. **Tải Dữ Liệu Từ Excel**
    - Mở Google Sheet mới của bạn.
    - Nhấn vào `Tệp` > `Nhập`.
    - Chọn `Tải lên` và chọn file Excel `QuanLyDiemDanh.xlsx` để nhập dữ liệu
    - Làm theo hướng dẫn để nhập dữ liệu vào Google Sheet.

## Bước 3: Chia Sẻ Google Sheet

1. **Chia Sẻ Với Tài Khoản Dịch Vụ**
    - Nhấn vào nút `Chia sẻ` ở góc trên bên phải của Google Sheet.
    - Trong trường `Chia sẻ với mọi người và nhóm`, nhập email:
        ```
        attendancesystem@attendancesystemapi-437006.iam.gserviceaccount.com
        ```
    - Đảm bảo quyền được đặt là `Người chỉnh sửa`.
    - Nhấn `Gửi`.

## Bước 4: Lấy ID Của Bảng Tính

1. **Sao Chép ID Của Bảng Tính**

    - Nhìn vào URL của Google Sheet. Nó sẽ trông giống như sau:
        ```
        https://docs.google.com/spreadsheets/d/SPREADSHEET_ID/edit#gid=0
        ```
    - Sao chép phần URL có dạng `SPREADSHEET_ID`. Đây là ID của bảng tính của bạn.

## Bước 5: Cấu Hình Mã Nguồn

1. **Thay Thế Tên Bảng và `spreadsheetId`**

    - Mở file `GoogleSheetsRepository.cs` trong thư mục `AttendanceManagementSystem\DAL`.
    - Tìm dòng sau và thay thế `QuanLyDiemDanh` ở dòng **38** bằng tên bảng của bạn.
        ```
        ApplicationName = "QuanLyDiemDanh"
        ```
    - Tìm và thay thế `YOUR_SPREADSHEET_ID` ở dòng **41** bằng `spreadsheetId` của bạn:

        ```
        _spreadsheetId = "YOUR_SPREADSHEET_ID";

        ```

---

# Cấu Trúc Cây Thư Mục

Dưới đây là cấu trúc cây thư mục của dự án và mô tả chức năng của từng phần:

```
- Properties
- References
- BLL (Business Logic Layer)
  - StudentBLL.cs
  - TeacherBLL.cs
  - UserBLL.cs
- DAL (Data Access Layer)
  - DataLoader.cs
  - GoogleSheetsRepository.cs
  - SQLiteRepository.cs
  - StudentDAL.cs
  - TeacherDAL.cs
  - UserDAL.cs
- Models
  - Announcements.cs
  - Courses.cs
  - Terms.cs
  - User.cs
  - Weeks.cs
- PL (Presentation Layer)
  - Admin
  - Login
    - ChangePassword.cs
    - ForgotPassword.cs
    - LoginForm.cs
    - MainForm.cs
  - Student
    - StudentCourses.cs
    - StudentWeeks.cs
  - Teacher
    - TeacherCourses.cs
    - TeacherWeeks.cs
  - MainDashboard.cs
- Resources
  - App.config
  - packages.config
- Program.cs
```

### Chức Năng Của Từng Phần

-   **Properties:** Chứa các thiết lập và thông tin cấu hình của dự án.
-   **References:** Chứa các tham chiếu đến các thư viện và gói cần thiết cho dự án.
-   **BLL (Business Logic Layer):**
    -   Chứa các lớp xử lý logic nghiệp vụ cho sinh viên, giáo viên và người dùng.
-   **DAL (Data Access Layer):**
    -   `DataLoader.cs`: Xử lý việc tải dữ liệu.
    -   `GoogleSheetsRepository.cs`: Quản lý kết nối và thao tác với Google Sheets.
    -   `SQLiteRepository.cs`: Quản lý kết nối và thao tác với cơ sở dữ liệu SQLite.
    -   `StudentDAL.cs`, `TeacherDAL.cs`, `UserDAL.cs`: Quản lý dữ liệu cho sinh viên, giáo viên và người dùng.
-   **Models:**
    -   Chứa các lớp mô hình dữ liệu, đại diện cho các thực thể như **User, Terms, Courses, ...**
-   **PL (Presentation Layer):**
    -   `Admin`, `Login`, `Student`, `Teacher`: Chứa các lớp giao diện người dùng cho từng phần chức năng.
    -   `MainDashboard.cs`: Giao diện chính của ứng dụng.
-   **Resources:**
    -   `App.config`: Cấu hình ứng dụng.
    -   `packages.config`: Danh sách các gói NuGet được sử dụng.
-   **Program.cs:**
    -   Điểm bắt đầu của ứng dụng, nơi khởi chạy chương trình.

---

# Cơ sở dữ liệu: Attendance_Management_System_CNPM

## Bảng Roles

-   **roleID**: Khóa chính, định danh vai trò.
-   **roleName**: Tên của vai trò (Sinh viên, Giáo viên, Admin).

## Bảng Users

-   **userID**: Khóa chính, định danh người dùng.
-   **username**: Tên đăng nhập, duy nhất.
-   **password**: Mật khẩu của người dùng.
-   **email**: Email của người dùng, duy nhất, định dạng hợp lệ.
-   **fullname**: Tên đầy đủ của người dùng.
-   **roleID**: Khóa ngoại, liên kết với bảng Roles.

## Bảng Terms

-   **termID**: Khóa chính, định danh kỳ học.
-   **termName**: Tên của kỳ học.
-   **startDate**: Ngày bắt đầu kỳ học.
-   **endDate**: Ngày kết thúc kỳ học.

## Bảng Courses

-   **courseID**: Khóa chính, định danh khóa học.
-   **courseName**: Tên của khóa học.
-   **courseCode**: Mã của khóa học.
-   **termID**: Khóa ngoại, liên kết với bảng Terms.

## Bảng CourseAssignments

-   **assignmentID**: Khóa chính, định danh phân công khóa học.
-   **courseID**: Khóa ngoại, liên kết với bảng Courses.
-   **teacherID**: Khóa ngoại, liên kết với bảng Users.

## Bảng Groups

-   **groupID**: Khóa chính, định danh nhóm.
-   **courseID**: Khóa ngoại, liên kết với bảng Courses.
-   **groupName**: Tên của nhóm.
-   **sessionTime**: Thời gian học của nhóm.
-   **classroom**: Phòng học của nhóm.

## Bảng Enrollments

-   **enrollmentID**: Khóa chính, định danh đăng ký.
-   **studentID**: Khóa ngoại, liên kết với bảng Users.
-   **groupID**: Khóa ngoại, liên kết với bảng Groups.

## Bảng Sessions

-   **sessionID**: Khóa chính, định danh buổi học.
-   **courseID**: Khóa ngoại, liên kết với bảng Courses.
-   **weekNumber**: Số tuần của buổi học.
-   **sessionDate**: Ngày diễn ra buổi học.

## Bảng Announcements

-   **announcementID**: Khóa chính, định danh thông báo.
-   **sessionID**: Khóa ngoại, liên kết với bảng Sessions.
-   **content**: Nội dung thông báo.

## Bảng AttendanceLinks

-   **linkID**: Khóa chính, định danh liên kết điểm danh.
-   **sessionID**: Khóa ngoại, liên kết với bảng Sessions.
-   **teacherID**: Khóa ngoại, liên kết với bảng Users.
-   **latitude**: Vĩ độ của vị trí điểm danh.
-   **longitude**: Kinh độ của vị trí điểm danh.

## Bảng Attendances

-   **attendanceID**: Khóa chính, định danh điểm danh.
-   **studentID**: Khóa ngoại, liên kết với bảng Users.
-   **sessionID**: Khóa ngoại, liên kết với bảng Sessions.
-   **status**: Trạng thái điểm danh (Có mặt, Vắng mặt).
-   **checkedInAt**: Thời gian điểm danh.
-   **latitude**: Vĩ độ của vị trí điểm danh.
-   **longitude**: Kinh độ của vị trí điểm danh.

## Cấu trúc thư mục

-   **BLL (Business Logic Layer):** Xử lý logic nghiệp vụ.
-   **DAL (Data Access Layer):** Truy cập dữ liệu.
-   **Models:** Mô hình dữ liệu.
-   **PL (Presentation Layer):** Giao diện người dùng.
-   **Resources:** Cấu hình và tài nguyên.
-   **Program.cs:** Điểm bắt đầu của ứng dụng.
