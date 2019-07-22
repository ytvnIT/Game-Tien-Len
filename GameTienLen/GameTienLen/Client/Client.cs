using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Client : Form
    {
        private TCPModel tcpForPlayer;
        private TCPModel tcpForOpponent;

        public Client(TCPModel player, TCPModel oppenent)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            tcpForPlayer = player;
            tcpForOpponent = oppenent;
           
        }    
       
        private void btnSearchRoom_Click(object sender, EventArgs e)
        {
            tcpForPlayer.SendData("timphong");
            //string t=f.tcpForPlayer.ReadData();
            string t = tcpForPlayer.ReadData();
            MessageBox.Show(t);

            DanhBai d = new DanhBai(tcpForPlayer, tcpForOpponent);
            this.Hide();
            d.ShowDialog();
            

            //tcpForPlayer.SendData(textBox1.Text);
            //textBox2.Text=tcpForOpponent.ReadData();
        }

        playSound ps = new playSound();
        private void button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt16(button1.Tag) == 0)
            {
                ps.OpenMediaFile("KiepDoDen-DuyManh.mp3");//ten bai hat.đinh dangcbai hat(bai hat phai di kem voi file exe)
                ps.PlayMediaFile(true);//hết bài dừng không lặp lại, true lặp lại
                button1.Tag = 1;
            }
            else
            {
                button1.Tag = 0;
                ps.ClosePlayer();
            }
        }
    }
}
