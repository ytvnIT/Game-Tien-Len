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
    }
}
