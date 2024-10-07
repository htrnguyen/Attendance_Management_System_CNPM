using Attendance_Management_System_CNPM.BLL;
using Attendance_Management_System_CNPM.Models;
using Attendance_Management_System_CNPM.PL.Login;
using Attendance_Management_System_CNPM.PL.Teacher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attendance_Management_System_CNPM.PL.Student
{
    public partial class MainDashboard : Form
    {
        private User user;
        private Terms terms;
        private readonly StudentBLL studentBLL;
        private readonly TeacherBLL teacherBLL;

        private int termID;
        public MainDashboard(User user)
        {
            InitializeComponent();
            this.user = user;
            this.studentBLL = new StudentBLL();
            this.teacherBLL = new TeacherBLL();
        }
        private void MainDashboard_Load(object sender, EventArgs e)
        {
            ShowUserInfo();
            ShowDateTime();
            GetTerms();
        }
        public void ShowForm(Form form)
        {
            panelMain.Controls.Clear();
            form.TopLevel = false;
            panelMain.Controls.Add(form);
            form.Show();
        }
        // Hiển thị thông tin người dùng lên panelUserInfo
        private void ShowUserInfo()
        {
            
            labelFullName.Text = user.FullName;
            labelRole.Text = user.Role;
        }
        // Hiển thị thứ, ngày, tháng, năm
        private void ShowDateTime()
        {
            string day = DateTime.Now.DayOfWeek.ToString();
            string date = DateTime.Now.Day.ToString();
            string month = DateTime.Now.Month.ToString();
            string year = DateTime.Now.Year.ToString();

            labelDateTime.Text = $"{day}, {date}/{month}/{year}";
        }
        // Lấy dữ liệu các kỳ học từ UserID
        public void GetTerms()
        {
            var userID = user.UserID;
            
            var terms = new List<Terms>();

            if (user.Role == "Sinh viên")
            {
                terms = studentBLL.GetTerms(userID);
            }
            else if (user.Role == "Giáo viên")
            {
                terms = teacherBLL.GetTerms(userID);
            }
            else if (user.Role == "Admin")
            {
                MessageBox.Show("Không có quyền truy cập", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (terms.Count == 0)
            {
                MessageBox.Show("Không có kỳ học nào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var term in terms)
            {
                comboBoxTerms.Items.Add(term.TermName);
                termID = term.TermID;
            }
            // Mặc định hiển thị kỳ học đầu tiên
            comboBoxTerms.SelectedIndex = 0;
            hiddenLabel.Focus();
        }

        private void comboBoxTerms_SelectedIndexChanged(object sender, EventArgs e)
        {
            hiddenLabel.Focus();
            // Lấy giá trị của kỳ học
            var termName = comboBoxTerms.SelectedItem.ToString();
            // Lấy ID của kỳ học
            var termID = studentBLL.GetTermID(termName);

            if (user.Role == "Sinh viên")
            {
                ShowForm(new StudentCourses(this, user.UserID, termID));
            }
            else if (user.Role == "Giáo viên")
            {
                ShowForm(new TeacherCourses(this, user.UserID, termID));
            }
        }

        private void labelLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
            this.Close();
        }

        private void labelDashboard_Click(object sender, EventArgs e)
        {
            comboBoxTerms_SelectedIndexChanged(sender, e);
        }
    }
}
