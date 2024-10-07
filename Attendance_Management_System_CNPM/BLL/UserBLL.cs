using AttendanceManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attendance_Management_System_CNPM.DAL;
using Attendance_Management_System_CNPM.Models;

namespace Attendance_Management_System_CNPM.BLL
{
    public class UserBLL
    {
        private readonly UserDAL userDAL;
        private readonly GoogleSheetsRepository _googleSheetsRepo;

        public UserBLL()
        {
            userDAL = new UserDAL();
            _googleSheetsRepo = new GoogleSheetsRepository();
        }
        // Login
        public bool IsLogin(string username, string password)
        {
            return userDAL.IsLogin(username, password);
        }
        // Forgot Password
        public bool IsEmailExist(string email)
        {
            return userDAL.IsEmailExist(email);
        }
        // Update Password
        public void UpdatePassword(string email, string password)
        {
            userDAL.UpdatePassword(email, password);
        }
        //Đồng bộ mật khẩu lên Google Sheets
        public async Task SyncPasswordToGoogleSheets()
        {
            await userDAL.SyncPasswordToGoogleSheets(_googleSheetsRepo);
        }
        // Lấy thông tin người dùng
        public async Task<User> GetUserInfo(string username)
        {
            return await userDAL.GetUserInfo(username);
        }
    }
}
