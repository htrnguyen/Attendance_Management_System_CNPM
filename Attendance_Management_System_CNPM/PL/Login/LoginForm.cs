using Attendance_Management_System_CNPM.BLL;
using Attendance_Management_System_CNPM.Models;
using Attendance_Management_System_CNPM.PL.Student;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attendance_Management_System_CNPM.PL.Login
{
    public partial class LoginForm : Form
    {
        private MainForm MainForm;
        private UserBLL _userBLL;
        public LoginForm(MainForm mainForm)
        {
            InitializeComponent();
            this.MainForm = mainForm;
            this._userBLL = new UserBLL();

            // Enter key to login
            tbUsername.KeyDown += tb_KeyDown;
            tbPassword.KeyDown += tb_KeyDown;
        }
        private void LoginForm_Load(object sender, EventArgs e)
        {
            HideError();
            tbUsername.Focus();
        }
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            HideError();

            // Test nhanh 
            //User user = await _userBLL.GetUserInfo("giaovien1");
            //User user = await _userBLL.GetUserInfo("sinhvien1");

            //this.Hide();
            //MainForm.Hide();
            //MainDashboard studentDashboard = new MainDashboard(user);
            //studentDashboard.ShowDialog();
            //// Nếu tắt form StudentDashboard thì tắt luôn form MainForm
            //this.MainForm.Close();
            //return;

            string username = tbUsername.Text;
            string password = tbPassword.Text;
            bool isValid = ValidateInput();

            if (!isValid)
            {
                return;
            }

            bool isLogin = _userBLL.IsLogin(username, password);

            if (isLogin)
            {
                User user = await _userBLL.GetUserInfo(username);

                if (user.Role == "Admin")
                {
                    MessageBox.Show("Đăng nhập với quyền Admin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                this.Hide();
                MainForm.Hide();
                MainDashboard studentDashboard = new MainDashboard(user);
                studentDashboard.ShowDialog();
                // Nếu tắt form MainDashboard thì tắt luôn form MainForm
                this.MainForm.Close();
            }
            else
            {
                ShowError("Tên đăng nhập hoặc mật khẩu không đúng");
            }
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(tbUsername.Text))
            {
                ShowError("Vui lòng nhập tên đăng nhập");
                return false;
            }
            if (string.IsNullOrEmpty(tbPassword.Text))
            { 
                ShowError("Vui lòng nhập mật khẩu");
                return false;
            }

            return true;
        }
        private void ShowError(string message)
        {
            pnError.Visible = true;
            pictureBoxError.Visible = true;
            lbMessageError.Text = message;
        }
        private void HideError()
        {
            pnError.Visible = false;
            pictureBoxError.Visible = false;
            lbMessageError.Text = "";
        }
        private void ShowPassword(object sender, EventArgs e)
        {
            picbHidePassword.Visible = true;
            picbShowPassword.Visible = false;
            tbPassword.UseSystemPasswordChar = false;
        }
        private void HidePassword(object sender, EventArgs e)
        {
            picbHidePassword.Visible = false;
            picbShowPassword.Visible = true;
            tbPassword.UseSystemPasswordChar = true;
        }

        private void lbForgotPassword_Click(object sender, EventArgs e)
        {
            MainForm.ShowForm(new ForgotPassword(MainForm));
        }
        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }
    }
}
