using Attendance_Management_System_CNPM.DAL;
using Attendance_Management_System_CNPM.Models;
using AttendanceManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance_Management_System_CNPM.BLL
{
    public class StudentBLL
    {
        private readonly StudentDAL studentDAL;
        private readonly GoogleSheetsRepository _googleSheetsRepo;

        public StudentBLL()
        {
            studentDAL = new StudentDAL();
            _googleSheetsRepo = new GoogleSheetsRepository();
        }
        // Lấy toàn bô kỳ học của sinh viên
        public List<Terms> GetTerms(int UserID)
        {
            return studentDAL.GetTerms(UserID);
        }
        // Lấy ID của kỳ học hiện tại
        public int GetTermID(string TermName)
        {
            return studentDAL.GetTermID(TermName);
        }
        // Lấy toàn bộ môn học của sinh viên
        public List<Courses> GetCourses(int UserID, int TermID)
        {
            return studentDAL.GetCourses(UserID, TermID);
        }
        // Lấy toàn bộ tuần học của môn học
        public List<Weeks> GetWeeks(int CourseID)
        {
            return studentDAL.GetWeeks(CourseID);
        }
        // Lấy toàn bộ thông báo của sinh viên
        public List<Announcements> GetAnnouncements(int WeekID)
        {
            return studentDAL.GetAnnouncements(WeekID);
        }
        // Kiểm tra giáo viên đã tạo link điểm danh chưa
        public bool CheckAttendanceLink(int WeekID, int CourseID, int TeacherID)
        {
            return studentDAL.CheckAttendanceLink(WeekID, CourseID, TeacherID);
        }
        // Lấy toạ độ lớp học
        public List<String> GetClassCoordinates(int ClassID)
        {
            return studentDAL.GetClassCoordinates(ClassID);
        }
    }
}
