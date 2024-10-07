using Attendance_Management_System_CNPM.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attendance_Management_System_CNPM.PL.Login
{
    public partial class ForgotPassword : Form
    {
        private MainForm MainForm;
        private UserBLL _userBLL;
        private string code;
        public ForgotPassword(MainForm mainForm)
        {
            InitializeComponent();
            this.MainForm = mainForm;
            this._userBLL = new UserBLL();

            tbCode.KeyPress += new KeyPressEventHandler(tbCode_KeyPress);

            // Enter để xác nhận
            tbEmail.KeyDown += new KeyEventHandler(tb_KeyDown);
            tbCode.KeyDown += new KeyEventHandler(tb_KeyDown);
        }

        private void ForgotPassword_Load(object sender, EventArgs e)
        {
            HideError();
            tbEmail.Focus();
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

        private void BackToLogin_Click(object sender, EventArgs e)
        {
            MainForm.ShowForm(new LoginForm(MainForm));
        }
        private bool ValidateInput()
        {
            // Kiểm tra email có rỗng không
            if (string.IsNullOrEmpty(tbEmail.Text))
            {
                ShowError("Vui lòng nhập email");
                return false;
            }
            // Kiểm tra email có đúng định dạng không
            if (!tbEmail.Text.Contains("@") || !tbEmail.Text.Contains("."))
            {
                ShowError("Email không hợp lệ");
                return false;
            }
            // Kiểm tra email có tồn tại không
            bool isEmailExist = _userBLL.IsEmailExist(tbEmail.Text);
            if (!isEmailExist)
            {
                ShowError("Email không tồn tại");
                return false;
            }
            return true;
        }
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            HideError();
            bool isValid = ValidateInput();
            if (!isValid)
            {
                return;
            }
            // Kiểm tra đã nhấn nút gửi mã chưa 
            if (code == null)
            {
                ShowError("Vui lòng nhấn nút gửi mã");
                return;
            }

            // Kiểm tra mã nhập vào có đúng không
            if (tbCode.Text != code)
            {
                ShowError("Mã không đúng");
                return;
            }

            // Chuyển sang ChangePassword
            MessageBox.Show("Mã xác nhận đúng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private string GenerateCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        private void SendVerificationEmail(string recipientEmail, string verificationCode)
        {
            ShowError("Đang gửi mã xác nhận...");
            var fromAddress = new MailAddress("hatrongnguyen04@gmail.com", "Hà Trọng Nguyễn");
            var toAddress = new MailAddress(recipientEmail);
            const string fromPassword = "vgws uaqj zgxj wywv";
            const string subject = "Mã xác nhận của bạn";

            // Định dạng lại nội dung email với HTML
            string body = $@"
            <html>
              <body>
                <h2 style='color: #333;'>Xin chào,</h2>
                <p style='font-size: 14px;'>Mã xác nhận của bạn là:</p>
                <p style='font-size: 20px; font-weight: bold; color: #4CAF50;'>{verificationCode}</p>
                <p>Nếu bạn không yêu cầu mã này, hãy bỏ qua email này.</p>
                <br />
                <p>Trân trọng,</p>
                <p>Đội ngũ hỗ trợ</p>
              </body>
            </html>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            try
            {
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
                ShowError("Đã gửi mã xác nhận qua email");
            }
            catch (SmtpException smtpEx)
            {
                ShowError($"Đã xảy ra lỗi khi gửi email: {smtpEx.Message}");
            }
            catch (FormatException formatEx)
            {
                ShowError($"Đã xảy ra lỗi khi gửi email: {formatEx.Message}");
            }
            catch (Exception ex)
            {
                ShowError($"Đã xảy ra lỗi khi gửi email: {ex.Message}");
            }
        }
        private void SendCode()
        {
            if (code == null)
            {
                code = GenerateCode();
            }
            SendVerificationEmail(tbEmail.Text, code);
        }

        private void lbNameReceiveCode_Click(object sender, EventArgs e)
        {
            bool isValid = ValidateInput();
            if (!isValid)
            {
                return;
            }

            SendCode();
        }

        private void picbReceiveCode_Click(object sender, EventArgs e)
        {
            bool isValid = ValidateInput();
            if (!isValid)
            {
                return;
            }

            SendCode();
        }
        // Chỉ có thể nhập 6 ký tự là số và nút backspace
        private void tbCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) || tbCode.Text.Length >= 6)
            {
                e.Handled = true;
            }
        }
        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConfirm_Click(sender, e);
            }
        }
    }
}
