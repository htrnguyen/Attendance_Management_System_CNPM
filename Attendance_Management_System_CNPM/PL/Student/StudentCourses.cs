﻿using Attendance_Management_System_CNPM.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attendance_Management_System_CNPM.PL.Student
{
    public partial class StudentCourses : Form
    {
        private readonly StudentBLL studentBLL;
        private MainDashboard mainDashboard;
        private int UserID;
        private int termID;

        public StudentCourses(MainDashboard mainDashboard, int userID, int termID)
        {
            InitializeComponent();
            this.mainDashboard = mainDashboard;
            this.studentBLL = new StudentBLL();
            this.UserID = userID;
            this.termID = termID;
        }
        private void StudentCourses_Load(object sender, EventArgs e)
        {
            GetCourses();
        }

        public void GetCourses()
        {
            var courses = studentBLL.GetCourses(UserID, termID);
            tlpMain.Controls.Clear();
            tlpMain.AutoScroll = true;
            tlpMain.AutoSize = false;
            tlpMain.ColumnStyles.Clear();
            tlpMain.RowStyles.Clear();

            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            Color[] courseColors = new Color[] {
                    Color.FromArgb(230, 243, 255),  // Light Blue
                    Color.FromArgb(255, 230, 230),  // Light Pink
                    Color.FromArgb(230, 255, 230),  // Light Green
                    Color.FromArgb(255, 255, 230)   // Light Yellow
                };

            for (int i = 0; i < courses.Count; i++)
            {
                var course = courses[i];
                var coursePanel = new TableLayoutPanel
                {
                    ColumnCount = 1,
                    RowCount = 4,
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    Margin = new Padding(0, 0, 0, 15), 
                    BackColor = courseColors[i % courseColors.Length],
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                    Tag = course // Thêm course vào Tag để lấy thông tin khi click vào course
                };

                var courseLabel = CreateLabel($"{course.CourseName} - {course.CourseCode}", true, Color.FromArgb(0, 102, 204));
                var groupLabel = CreateLabel($"Group: {course.GroupName}", false, Color.Black);
                var timeLabel = CreateLabel($"Time: {course.SessionTime}", false, Color.Black);
                var roomLabel = CreateLabel($"Room: {course.ClassRoom}", false, Color.Black);

                courseLabel.Cursor = Cursors.Hand;
                groupLabel.Cursor = Cursors.Hand;
                timeLabel.Cursor = Cursors.Hand;
                roomLabel.Cursor = Cursors.Hand;

                // Add click event handler
                coursePanel.Click += CoursePanel_Click;
                courseLabel.Click += CoursePanel_Click;
                groupLabel.Click += CoursePanel_Click;
                timeLabel.Click += CoursePanel_Click;
                roomLabel.Click += CoursePanel_Click;

                coursePanel.Controls.Add(courseLabel, 0, 0);
                coursePanel.Controls.Add(groupLabel, 0, 1);
                coursePanel.Controls.Add(timeLabel, 0, 2);
                coursePanel.Controls.Add(roomLabel, 0, 3);

                tlpMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlpMain.Controls.Add(coursePanel, 0, i);
            }
        }

        private void CoursePanel_Click(object sender, EventArgs e)
        {
            TableLayoutPanel coursePanel;

            // Kiểm tra xem sender có phải là TableLayoutPanel không
            if (sender is TableLayoutPanel)
            {
                coursePanel = (TableLayoutPanel)sender;
            }
            else if (sender is Label)
            {
                // Nếu sender là Label, lấy TableLayoutPanel cha của nó
                coursePanel = (TableLayoutPanel)((Label)sender).Parent;
            }
            else
            {
                // Nếu sender không phải TableLayoutPanel hoặc Label, thoát khỏi phương thức
                return;
            }

            var course = (Models.Courses)coursePanel.Tag;

            StudentWeeks studentWeeks = new StudentWeeks(mainDashboard, UserID, course);
            mainDashboard.ShowForm(studentWeeks);
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
    }
}
