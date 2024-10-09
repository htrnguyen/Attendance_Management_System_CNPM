using AttendanceManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attendance_Management_System_CNPM.PL.Login
{
    public partial class MainForm : Form
    {
        private bool isLoading = false;
        private DataLoader _dataLoader;
        private PictureBox pictureBoxLoad;
        public MainForm()
        {
            InitializeComponent();

            // Khởi tạo DataLoader
            var googleSheetsRepo = new GoogleSheetsRepository();
            var sqliteRepo = new SQLiteRepository();
            //_dataLoader = new DataLoader(googleSheetsRepo, sqliteRepo);
            _dataLoader = new DataLoader();

            // Khởi tạo PictureBox loading
            InitializeLoadingPictureBox();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                isLoading = true;

                // Ẩn các panel và PictureBox
                panelMain.Visible = false;
                pictureBox.Visible = false;

                // Hiện PictureBox loading
                pictureBoxLoad.Visible = true;

                // Tải dữ liệu
                await StartLoadingData();

                // Ẩn PictureBox loading
                pictureBoxLoad.Visible = false;

                // Hiện lại các panel
                panelMain.Visible = true;
                pictureBox.Visible = true;

                isLoading = false;
            }

            // Hiển thị Form Login lên panelMain
            ShowForm(new LoginForm(this));
            //ShowForm(new ChangePassword(this, "hatrongnguyen0423@gmail.com"));
        }
        // Tải dữ liệu
        public async Task StartLoadingData()
        {
            await _dataLoader.LoadDataAsync();
        }
        private void InitializeLoadingPictureBox()
        {
            string imagePath = Path.Combine(Application.StartupPath, @"..\..\Resources\images", "loading3.gif");
            if (File.Exists(imagePath))
            {
                pictureBoxLoad = new PictureBox
                {
                    Image = Image.FromFile(imagePath),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Fill,
                    Visible = false
                };
                this.Controls.Add(pictureBoxLoad);
            }
            else
            {
                MessageBox.Show("Loading image not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Hiển thị Form lên panelMain
        public void ShowForm(Form form)
        {
            panelMain.Controls.Clear();
            form.TopLevel = false;
            panelMain.Controls.Add(form);
            form.Show();
        }
    }
}
