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
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Windows.Forms;
using System.IO;


namespace Attendance_Management_System_CNPM.PL.Teacher
{
    public partial class TeacherWeeks : Form
    {
        private readonly TeacherBLL teacherBLL;
        private MainDashboard mainDashboard;
        private int UserID;
        private Courses courses;
        private List<Weeks> Weeks;
        private int weekIDTemp;
        private WebView2 webView;

        private string receivedLat;
        private string receivedLon;
        public TeacherWeeks(MainDashboard mainDashboard, int userID, Courses courses)
        {
            InitializeComponent();
            InitializeWebView();
            this.mainDashboard = mainDashboard;
            this.teacherBLL = new TeacherBLL();
            this.UserID = userID;
            this.courses = courses;

            this.receivedLat = "-1";
            this.receivedLon = "-1";
        }
        private async void InitializeWebView()
        {
            webView = new WebView2();
            webView.Size = new System.Drawing.Size(400, 200); // Thiết lập kích thước cho WebView2
            webView.Location = new System.Drawing.Point(10, 60); // Vị trí của WebView2
            this.Controls.Add(webView);
            await webView.EnsureCoreWebView2Async(null);

            // Lắng nghe sự kiện WebMessageReceived
            webView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;
        }
        private async void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            // Lấy thông điệp từ JavaScript
            string message = e.TryGetWebMessageAsString();

            // Phân tích cú pháp JSON
            try
            {
                // Chuyển đổi chuỗi JSON thành đối tượng Dictionary
                var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, double>>(message);

                // Lấy tọa độ từ đối tượng đã phân tích
                this.receivedLat = jsonData["latitude"].ToString();
                this.receivedLon = jsonData["longitude"].ToString();

                // Cập nhật tọa độ lên Google Sheets trong bảng Class 
                if (receivedLat != "-1" && receivedLon != "-1")
                {     
                    teacherBLL.UpdateCoordinates(courses.ClassID, courses.CourseID, receivedLat, receivedLon);
                    MessageBox.Show("Cập nhật tọa độ thành công!");

                    if (teacherBLL.UpdateAttendanceLinkStatus(weekIDTemp, courses.CourseID, UserID))
                    {
                        MessageBox.Show("Cập nhật trạng thái điểm danh thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật trạng thái điểm danh thất bại!");
                    }
                    
                    await teacherBLL.UpdateClassesToGoogleSheet();
                    await teacherBLL.UpdateWeeksToGoogleSheet();

                    GetWeeks();

                }
                else
                {
                    MessageBox.Show("Cập nhật tọa độ thất bại!");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu không phân tích được JSON
                MessageBox.Show($"Lỗi phân tích JSON: {ex.Message}");
            }
        }
        private void TeacherWeeks_Load(object sender, EventArgs e)
        {
            GetWeeks();
        }
        private void GetWeeks()
        {
            this.Weeks = teacherBLL.GetWeeks(courses.CourseID);

            tlpMain.Controls.Clear();
            tlpMain.AutoScroll = true;
            tlpMain.AutoSize = false;
            tlpMain.ColumnStyles.Clear();
            tlpMain.RowStyles.Clear();

            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            Color panelColor = Color.FromArgb(242, 244, 250);

            for (int i = 0; i < Weeks.Count; i++)
            {
                var week = Weeks[i];
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

                var actionButton = new Button
                {
                    AutoSize = true,
                    Margin = new Padding(5),
                    Padding = new Padding(5),
                    MinimumSize = new Size(50, 25) // Set a small size for the button
                };

                bool isLinkCreated = teacherBLL.CheckAttendanceLink(week.WeekID, courses.CourseID, UserID);
                if (isLinkCreated)
                {
                    actionButton.Text = "Đã tạo";
                    actionButton.Enabled = false; 
                }
                else
                {
                    actionButton.Text = "Tạo link";
                    actionButton.Click += (s, e) => GetLocation_Click(week.WeekID);
                }

                weekPanel.Controls.Add(actionButton, 0, announcements.Count + 1);

                tlpMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlpMain.Controls.Add(weekPanel, 0, i);
            }
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
        private void GetLocation_Click(int weekID)
        {
            this.weekIDTemp = weekID;
            GetLocation();
        }
        // Hàm lấy toạ độ (kinh độ, vĩ độ)
        public void GetLocation()
        {
            string projectRoot = Directory.GetParent(Application.StartupPath).Parent.FullName;
            string filePath = Path.Combine(projectRoot, "DiemDanh", "DiemDanh.html");
            webView.Source = new Uri(filePath);

            // Đảm bảo trang web đã tải xong trước khi inject JavaScript
            webView.CoreWebView2.NavigationCompleted += async (s, args) =>
            {
                if (args.IsSuccess)
                {
                    // Inject và gọi hàm JavaScript getLocation() tự động
                    await webView.CoreWebView2.ExecuteScriptAsync("getLocation();");
                }
            };
        }
    }
}
