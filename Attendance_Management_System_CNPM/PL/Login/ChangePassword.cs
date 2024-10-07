using Attendance_Management_System_CNPM.BLL;
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
    public partial class ChangePassword : Form
    {
        private readonly MainForm mainForm;
        private readonly string email;
        private UserBLL userBLL;
        public ChangePassword(MainForm mainForm, string email)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.email = email;
            this.userBLL = new UserBLL();

            // Enter to change password
            tbNewPassword.KeyDown += tb_KeyDown;
            tbConfirmPassword.KeyDown += tb_KeyDown;
        }
        private void ChangePassword_Load(object sender, EventArgs e)
        {
            HideError();
            tbNewPassword.Focus();

        }
        private void HideError()
        {
            pnError.Visible = false;
            pictureBoxError.Visible = false;
            lbMessageError.Text = "";
        }
        private void ShowError(string message)
        {
            pnError.Visible = true;
            pictureBoxError.Visible = true;
            lbMessageError.Text = message;
        }
        private bool ValidateInput()
        { 
            if (string.IsNullOrEmpty(tbNewPassword.Text))
            {
                ShowError("Vui lòng nhập mật khẩu mới");
                return false;
            }
            if (string.IsNullOrEmpty(tbConfirmPassword.Text))
            {
                ShowError("Vui lòng xác nhận mật khẩu");
                return false;
            }
            if (tbNewPassword.Text != tbConfirmPassword.Text)
            {
                ShowError("Mật khẩu xác nhận không khớp");
                return false;
            }
            return true;
        }
        private async void SyncPasswordToGoogleSheets()
        {
            await userBLL.SyncPasswordToGoogleSheets();
        }
        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }
            userBLL.UpdatePassword(email, tbNewPassword.Text);
            SyncPasswordToGoogleSheets();

            // Thông báo và chuyển về form đăng nhập sau khi nhân Ok
            MessageBox.Show("Đổi mật khẩu thành công.\nQuay về trang đăng nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            mainForm.ShowForm(new LoginForm(mainForm));
        }
        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnChangePassword_Click(sender, e);
            }
        }
    }
}
