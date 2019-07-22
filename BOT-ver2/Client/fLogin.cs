using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        }

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
        }

      

        private void fLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            tcpForPlayer.CloseConnection();
            tcpForOpponent.CloseConnection();
            if (MessageBox.Show("Bạn có thực sự muốn thoát chương trình?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                e.Cancel = true;
        }
    }
}
