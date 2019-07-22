using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace Client
{
    public partial class fLogin : Form
    {
        public TCPModel tcpForPlayer;
        public TCPModel tcpForOpponent;

        public fLogin()
        {
            InitializeComponent();
            // Connect();
            ps.OpenMediaFile("KiepDoDen-DuyManh.mp3");
            ps.PlayMediaFile(true);

        }
        playSound ps = new playSound();
        public void Connect()
        {
            string ip = "127.0.0.1";
            int port = 13000;

            tcpForPlayer = new TCPModel(ip, port);
            tcpForPlayer.ConnectToServer();
            this.Text = tcpForPlayer.UpdateInformation();

            tcpForOpponent = new TCPModel(ip, port);
            tcpForOpponent.ConnectToServer();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Connect();
            tcpForPlayer.SendData("dangnhap");
            string t=tcpForPlayer.ReadData();
            MessageBox.Show(t);

            Client c = new Client(tcpForPlayer,tcpForOpponent);
 
            this.Hide();
            c.ShowDialog();
            this.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
            ps.ClosePlayer();
        }

      

        private void fLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có thực sự muốn thoát chương trình?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                e.Cancel = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(80, 0, 0, 0);
            //WindowsMediaPlayer sound = new WindowsMediaPlayer();
            //sound.URL = @"C:\Users\DELL\Downloads\Tove-Lo-Habits-Stay-High-Hippie-Sabotage-Remix.mp3";
            //sound.controls.play();
        }
    }
}
