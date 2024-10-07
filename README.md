## **1. Tóm tắt yêu cầu chức năng chính**

### **1.1. Quản lý người dùng**

-   **Sinh viên** và **giáo viên** đều có tài khoản để truy cập hệ thống.
-   Người dùng có thể xem các **kỳ học** mà họ đã tham gia hoặc đã dạy.

### **1.2. Quản lý kỳ học**

-   Trong mỗi kỳ học, người dùng có thể xem các **môn học** họ đã học hoặc đã dạy.

### **1.3. Quản lý môn học**

-   Trong mỗi môn học, người dùng có thể xem chi tiết của từng **tuần học**.
-   Trong mỗi tuần học, người dùng có thể xem các **thông báo** và **link điểm danh**.

### **1.4. Giáo viên tạo link điểm danh**

-   Giáo viên có thể tạo một link điểm danh cho một tuần học cụ thể.
-   Hệ thống sẽ ghi nhận vị trí (kinh độ và vĩ độ) của giáo viên khi tạo link.

### **1.5. Sinh viên kiểm tra vị trí**

-   Sinh viên sẽ nhận được link điểm danh và khi truy cập, hệ thống sẽ ghi nhận vị trí hiện tại của họ (kinh độ và vĩ độ).
-   Hệ thống sẽ so sánh vị trí của sinh viên với vị trí của giáo viên để xác định xem sinh viên có đủ điều kiện để điểm danh hay không.

### **1.6. Điểm danh**

-   Nếu sinh viên ở trong khoảng cách cho phép (ví dụ: 100 mét), họ sẽ được đánh dấu là có mặt.
-   Nếu không, họ sẽ được đánh dấu là vắng mặt.

## **2. Thiết kế cơ sở dữ liệu**

Dựa trên các yêu cầu trên, dưới đây là thiết kế cơ sở dữ liệu với các bảng và mối quan hệ giữa chúng.

### **2.1. Sơ đồ ER (Entity-Relationship Diagram)**

-   **Users** (Người dùng)

    -   Một người dùng có thể là **Sinh viên**, **Giáo viên**, hoặc **Admin**.

-   **Roles** (Vai trò)

    -   Các vai trò như **Sinh viên**, **Giáo viên**, **Admin**.

-   **Terms** (Kỳ học)

    -   Mỗi kỳ học có nhiều **Môn học**.

-   **Courses** (Môn học)

    -   Mỗi môn học thuộc về một kỳ học.
    -   Một môn học có thể được **giảng dạy bởi nhiều giáo viên**.
    -   Một môn học có nhiều **Nhóm học**.

-   **CourseTeachers** (Giảng viên dạy môn học)

    -   Liên kết giữa **Giáo viên** và **Môn học** (Many-to-Many).

-   **Groups** (Nhóm học)

    -   Mỗi nhóm học thuộc về một môn học.

-   **Enrollments** (Đăng ký môn học)

    -   Liên kết giữa **Sinh viên** và **Môn học**, bao gồm **Nhóm học** mà sinh viên tham gia.

-   **Sessions** (Tuần học)

    -   Mỗi tuần học thuộc về một môn học.

-   **Announcements** (Thông báo)

    -   Mỗi thông báo thuộc về một tuần học cụ thể.

-   **Attendances** (Điểm danh)

    -   Ghi nhận điểm danh của sinh viên trong từng tuần học.

-   **AttendanceLinks** (Link điểm danh)

    -   Lưu trữ các link điểm danh mà giáo viên tạo ra.
