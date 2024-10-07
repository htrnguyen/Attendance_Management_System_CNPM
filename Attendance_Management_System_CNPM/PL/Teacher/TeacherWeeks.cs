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

namespace Attendance_Management_System_CNPM.PL.Teacher
{
    public partial class TeacherWeeks : Form
    {
        private readonly TeacherBLL teacherBLL;
        private MainDashboard mainDashboard;
        private int UserID;
        private Courses courses;
        public TeacherWeeks(MainDashboard mainDashboard, int userID, Courses courses)
        {
            InitializeComponent();
            this.mainDashboard = mainDashboard;
            this.teacherBLL = new TeacherBLL();
            this.UserID = userID;
            this.courses = courses;
        }

        private void TeacherWeeks_Load(object sender, EventArgs e)
        {
            GetWeeks();
        }
        private void GetWeeks()
        {
            var weeks = teacherBLL.GetWeeks(courses.CourseID);

            tlpMain.Controls.Clear();
            tlpMain.AutoScroll = true;
            tlpMain.AutoSize = false;
            tlpMain.ColumnStyles.Clear();
            tlpMain.RowStyles.Clear();

            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            Color panelColor = Color.FromArgb(242, 244, 250);

            for (int i = 0; i < weeks.Count; i++)
            {
                var week = weeks[i];
                var announcements = teacherBLL.GetAnnouncements(week.WeekID);

                var weekPanel = new TableLayoutPanel
                {
                    ColumnCount = 1,
                    RowCount = 1 + announcements.Count + 1,
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    Margin = new Padding(0, 0, 0, 15),
                    BackColor = panelColor,
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                    Tag = week
                };

                var weekLabel = CreateLabel($"Tuần {week.WeekNumber}", true, Color.FromArgb(0, 102, 204));
                weekPanel.Controls.Add(weekLabel, 0, 0);

                for (int j = 0; j < announcements.Count; j++)
                {
                    var announcement = announcements[j];
                    var announcementLabel = CreateLabel(announcement.Content, false, Color.Black);
                    weekPanel.Controls.Add(announcementLabel, 0, j + 1);
                }

                // Check if the attendance link has been created
                bool isLinkCreated = CheckAttendanceLink(week.WeekID, courses.CourseID, UserID);

                var actionButton = new Button
                {
                    AutoSize = true,
                    Margin = new Padding(5),
                    Padding = new Padding(5),
                    MinimumSize = new Size(50, 25) // Set a small size for the button
                };

                if (isLinkCreated)
                {
                    actionButton.Text = "Đã tạo";
                    actionButton.Enabled = false; // Disable the button if the link is already created
                }
                else
                {
                    actionButton.Text = "Tạo link";
                    actionButton.Click += (s, e) => CreateLinkButton_Click(week.WeekID);
                }

                weekPanel.Controls.Add(actionButton, 0, announcements.Count + 1);

                tlpMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlpMain.Controls.Add(weekPanel, 0, i);
            }
        }

        private void CreateLinkButton_Click(int weekID)
        {
            MessageBox.Show($"Kiêt Chung làm ...");
        }           

        private Label CreateLabel(string text, bool isTitle = false, Color textColor = default)
        {
            var label = new Label
            {
                Text = text,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Arial", isTitle ? 14 : 10, isTitle ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = textColor,
                AutoEllipsis = true,
                Margin = new Padding(2),
                Padding = new Padding(5)
            };

            return label;
        }

        // Kiểm tra giáo viên đã tạo link điểm danh chưa
        public bool CheckAttendanceLink(int sessionID, int courseID, int teacherID)
        {
            return teacherBLL.CheckAttendanceLink(sessionID, courseID, teacherID);
        }
    }
}
