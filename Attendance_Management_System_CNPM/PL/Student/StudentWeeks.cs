using Attendance_Management_System_CNPM.BLL;
using Attendance_Management_System_CNPM.Models;
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
using Attendance_Management_System_CNPM.DAL;
using System.IO;
using System.Net;

namespace Attendance_Management_System_CNPM.PL.Student
{
    public partial class StudentWeeks : Form
    {
        private readonly StudentBLL studentBLL;
        private MainDashboard mainDashboard;
        private int UserID;
        private Courses courses;
        private WebView2 webView;


        private string receivedLat;
        private string receivedLon;
        public StudentWeeks(MainDashboard mainDashboard, int userID, Courses courses)
        {
            InitializeComponent();
            InitializeWebView();
            this.mainDashboard = mainDashboard;
            this.studentBLL = new StudentBLL();
            this.UserID = userID;
            this.courses = courses;
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

                var coordinates = studentBLL.GetClassCoordinates(courses.ClassID);
                string class_latitude = coordinates[0];
                string class_longitude = coordinates[1];

                // Ép kiểu thành double 
                double class_lat = Convert.ToDouble(class_latitude);
                double class_lon = Convert.ToDouble(class_longitude);
                double received_lat = Convert.ToDouble(this.receivedLat);
                double received_lon = Convert.ToDouble(this.receivedLon);

                MessageBox.Show($"Tọa độ lớp học: {class_lat}, {class_lon}\nTọa độ nhận được: {received_lat}, {received_lon}");

                // Kiểm tra tọa độ có nằm trong bán kính không
                bool isInArea = IsPointInRadius(class_lat, class_lon, received_lat, received_lon, 10.0);

                // Hiển thị kết quả lên label
                if (isInArea)
                {
                    MessageBox.Show("Kết quả kiểm tra: Tọa độ nằm trong khu vực.");
                }
                else
                {
                    MessageBox.Show("Kết quả kiểm tra: Tọa độ nằm ngoài khu vực.");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu không phân tích được JSON
                MessageBox.Show($"Lỗi phân tích JSON: {ex.Message}");
            }
        }
        private void StudentWeeks_Load(object sender, EventArgs e)
        {
            GetWeeks();
        }
        // Lấy toàn bộ tuần học của môn học
        private void GetWeeks()
        {
            var weeks = studentBLL.GetWeeks(courses.CourseID);

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
                var announcements = studentBLL.GetAnnouncements(week.WeekID);

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

                var checkInButton = new Button
                {
                    Text = "Điểm danh",
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    AutoSize = true,
                    Margin = new Padding(5),
                    Padding = new Padding(5),
                    MinimumSize = new Size(80, 25) 
                };
                checkInButton.Click += (s, e) => CheckInButton_Click(week.WeekID);

                if (studentBLL.CheckAttendanceLink(week.WeekID, courses.CourseID, courses.TeacherID))
                {
                    checkInButton.Enabled = true;
                }
                else
                {
                    checkInButton.Text = "Đã điểm danh";
                    checkInButton.Enabled = false;
                }

                weekPanel.Controls.Add(checkInButton, 0, announcements.Count + 1);

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

        // Kiểm tra giáo viên đã tạo link điểm danh chưa
        private void CheckInButton_Click(int weekID)
        {
            string ip = GetIPAddress();
            MessageBox.Show(ip);

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
        private double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double earthRadius = 6371000; // Bán kính Trái Đất tính bằng mét
            double dLat = (lat2 - lat1) * (Math.PI / 180);
            double dLon = (lon2 - lon1) * (Math.PI / 180);

            lat1 = lat1 * (Math.PI / 180);
            lat2 = lat2 * (Math.PI / 180);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = earthRadius * c;
            MessageBox.Show($"Khoảng cách giữa 2 điểm là: {distance} mét");
            return distance;
        }

        // Hàm kiểm tra xem tọa độ có nằm trong bán kính hay không
        private bool IsPointInRadius(double centerLat, double centerLon, double checkLat, double checkLon, double radiusInMeters)
        {
            double distance = HaversineDistance(centerLat, centerLon, checkLat, checkLon);
            return distance <= radiusInMeters;
        }
        private static string GetIPAddress()
        {
            string IPAddress = string.Empty;
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }
            return IPAddress;
        }
    }
}

