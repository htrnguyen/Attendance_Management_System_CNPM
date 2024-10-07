﻿using Attendance_Management_System_CNPM.DAL;
using Attendance_Management_System_CNPM.Models;
using AttendanceManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance_Management_System_CNPM.BLL
{
    public class TeacherBLL
    {
        private readonly TeacherDAL teacherDAL;
        private readonly GoogleSheetsRepository _googleSheetsRepo;

        public TeacherBLL()
        {
            teacherDAL = new TeacherDAL();
            _googleSheetsRepo = new GoogleSheetsRepository();
        }
        // Lấy toàn bộ kỳ học của giáo viên
        public List<Terms> GetTerms(int UserID)
        {
            return teacherDAL.GetTerms(UserID);
        }
        // Lấy toàn bộ môn học của giáo viên
        public List<Courses> GetCourses(int UserID, int TermID)
        {
            return teacherDAL.GetCourses(UserID, TermID);
        }
        // Lấy toàn bộ tuần học của môn học
        public List<Weeks> GetWeeks(int CourseID)
        {
            return teacherDAL.GetWeeks(CourseID);
        }
        // Lấy toàn bộ thông báo của giáo viên
        public List<Announcements> GetAnnouncements(int WeekID)
        {
            return teacherDAL.GetAnnouncements(WeekID);
        }
        // Kiểm tra xem giáo viên đã điểm danh cho tuần học đó chưa
        public bool CheckAttendanceLink(int sessionID, int courseID, int teacherID)
        {
            return teacherDAL.CheckAttendanceLink(sessionID, courseID, teacherID);
        }
    }
}
