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
    public partial class DanhBai : Form
    {
        private TCPModel tcpForPlayer;
        private TCPModel tcpForOpponent;
        private List<double> BaiHienTai = new List<double>();
        private List<double> BaiCuaDoiThu = new List<double>();
        int Y;
        int X;
        int[] op;//Mảng dùng để lưu STT của đối thủ trong bàn
        string text0 = "";
        string text1 = "";
        string text2 = "";
        int Myturn;
        int players;
        List<PictureBox> pictureBoxList1 = new List<PictureBox>();
        List<PictureBox> pictureBoxList2 = new List<PictureBox>();
        List<PictureBox> pictureBoxList3 = new List<PictureBox>();
        List<string> stringList = new List<string>();
        List<Label> labelList = new List<Label>();
        string String_Fight;
     

        string path = @"C:\Users\ASUS\Desktop\BOT-ver2\lastversion\Ver-chót2\GameTienLen\GameTienLen\Client\Resources\";
        public DanhBai(TCPModel player, TCPModel Opponent)
        {
            InitializeComponent();
            tcpForPlayer = player;
            tcpForOpponent = Opponent;
            btnDanh.Enabled = false;
            btnBoLuot.Enabled = false;
            if (tcpForPlayer.ReadData() == "isplaying")
                btnReady.Enabled = false;

            Y = pictureBox1.Location.Y;

            addPictureBoxList(pictureBoxList1, 1);
            addPictureBoxList(pictureBoxList2, 14);
            pictureBoxList3.Add(pictureBox28);
            pictureBoxList3.Add(pictureBox29);
            pictureBoxList3.Add(pictureBox30);

            stringList.Add(text0);
            stringList.Add(text1);
            stringList.Add(text2);

            labelList.Add(label2);
            labelList.Add(label3);
            labelList.Add(label4);

            for(int i=0;i<players-1;i++)
            {
                stringList[i] = "";
            }

            String_Fight = "";
            Thread t = new Thread(NhanBai);
            t.Start();
        }

        void addPictureBoxList(List<PictureBox> list, int n)
        {
            Control[] matches;
            for (int i = n; i <= n + 12; i++)
            {
                matches = this.Controls.Find("pictureBox" + i.ToString(), true);
                if (matches.Length > 0 && matches[0] is PictureBox)
                {
                    list.Add((PictureBox)matches[0]);
                }
            }
        }

        void ResetPictureBoxList2(int n)
        {
            for (int i = 0; i < n; i++)
            {
                //pictureBoxList2[i].Image = null;
                //pictureBoxList2[i].BackColor = DefaultBackColor;
                //pictureBoxList2[i].Tag = null;
                pictureBoxList2[i].Refresh();
                pictureBoxList2[i].Visible = false;
            }
        }
        void ResetPictureBoxList1(int n)
        {
            for (int i = 0; i < n; i++)
            {
                pictureBoxList1[i].Image = null;
                pictureBoxList1[i].BackColor = DefaultBackColor;
                pictureBoxList1[i].Tag = null;
            }
        }

        void effect(PictureBox pic,int n)
        {
            pic.BackgroundImage = null;
            pic.Image = Image.FromFile(path+"Player"+n.ToString()+".jpg");
            pic.BackColor = Color.Yellow;
            pic.Padding = new Padding(2);
        }

        void resetEffect(PictureBox pic,int n)
        {
            pic.BackgroundImage = null;
            pic.Image = Image.FromFile(path + "Player" + n.ToString() + ".jpg");
            pic.Padding = new Padding(0);
        }

        void effectMatLuot(PictureBox pic,int n)
        {
            pic.BackgroundImage = Image.FromFile(path + "Player" + n.ToString() + ".jpg");
            pic.Image = Image.FromFile(path + "stop3.png");
            pic.Padding = new Padding(0);
        }
        int signal = 0;
        private void btnReady_Click(object sender, EventArgs e)
        {
            btnBoLuot.Visible = true;
            btnDanh.Visible = true;
            for (int i = 0; i < players - 1; i++)
            {
                stringList[i] = "";
            }
            //btnDanh.Visible = true;
            //btnBoLuot.Visible = true;
            
            tcpForPlayer.SendData("chiabai");
            string result = tcpForPlayer.ReadData();
            if(result.Last() == '@')
            {
                signal = 1;
                result = result.Remove(result.Length - 1);
            }
            //Nếu người chơi nhận được lượt đánh thì enable btbDanh
            players = Convert.ToInt32(result[result.Length - 2].ToString()); //Số người trong phòng hiện tại
            

            op = new int[3] { -1, -1, -1 };
            Myturn = Convert.ToInt16(result.Last().ToString());

            op[0] = (Myturn + 1) % players;
            for (int i = 1; i < players - 1; i++)
                op[i] = (op[i - 1] + 1) % players;

            for (int i = 0; i < players-1; i++)
            {
                if (op[i] > -1)
                {
                    resetEffect(pictureBoxList3[i], op[i]);
                }
            }
            pictureBox27.Visible = true;
            pictureBox27.Image = Image.FromFile(path + "Player" + Myturn.ToString() + ".jpg");
            label1.Visible = true;
            label1.Text = "Player " + Myturn.ToString();

            for (int i=0;i<players-1;i++)
            {
                pictureBoxList3[i].Visible = true;
                pictureBoxList3[i].Image = Image.FromFile(path + "Player" + op[i].ToString() + ".jpg");
                labelList[i].Visible = true;
                labelList[i].Text = "Player " + op[i].ToString();
            }



            if (result.Contains("turn"))
            {
                result = result.Remove(result.Length - 6);//Xóa 6 kí tự cuối chuối(là string "turni")
                //btnBoLuot.Visible = true;
                btnDanh.Enabled = true;
                effect(pictureBox27,Myturn);
            }
            else
            {
                int turn = Convert.ToInt16(result[result.Length - 3].ToString());
                for(int i=0;i<players-1;i++)
                {
                    if (turn == op[i]&&op[i]>-1)
                        effect(pictureBoxList3[i],op[i]);
                }
                result = result.Remove(result.Length - 3);
            }
            ConvertToListDouble(BaiHienTai, result);//Convert từ string sang List<double>
            //Kiểm tra xem bài của người cho có tới trắng hay không
            if (XuLyBai.KiemTraToiTrang(BaiHienTai) == true)
                tcpForPlayer.SendData(ConvertToString(BaiHienTai) + "wintrang");

            ResetPictureBoxList2(13);

            /*txbBai.Text = ConvertToString(BaiHienTai);*///gọi hàm load hình LoadHinh(BaiHienTai)  
            for (int i = 0; i < BaiHienTai.Count(); i++)
            {
                pictureBoxList1[i].Visible = true;
                pictureBoxList1[i].Image = Image.FromFile(path + BaiHienTai[i].ToString() + ".png");
                pictureBoxList1[i].BackColor = Color.White;
                pictureBoxList1[i].BorderStyle = BorderStyle.FixedSingle;
                pictureBoxList1[i].Tag = BaiHienTai[i];
            }
        }

        void NhanBai()//Dùng để nhận bài của đối thủ
        {
            while (true)
            {
                try
                {
                    string result = tcpForOpponent.ReadData();//Bài của đối thủ vừa đánh
                                                              //Nếu nhận được "newturn" thì người chơi có quyền đánh những lá tùy ý.
                    if (result.Contains("newturn"))
                    {
                        effect(pictureBox27,Myturn);
                        if (op[0] == Convert.ToInt16(result.Last().ToString()))
                        {
                            effectMatLuot(pictureBox28,op[0]);
                            stringList[0] = result.Last().ToString() + "boluot";
                        }
                        if (op[1] == Convert.ToInt16(result.Last().ToString()))
                        {
                            effectMatLuot(pictureBox29,op[1]);
                            stringList[1] = result.Last().ToString() + "boluot";
                        }
                        if (op[2] == Convert.ToInt16(result.Last().ToString()))
                        {
                            effectMatLuot(pictureBox30,op[2]);
                            stringList[2] = result.Last().ToString() + "boluot";
                        }

                        Thread.Sleep(1000);

                        for (int i = 0; i < players-1; i++)
                        {
                            if (op[i] > -1)
                            {
                                resetEffect(pictureBoxList3[i], op[i]);
                            }
                        }

                        BaiCuaDoiThu.Clear();
                        for(int i=0;i<players-1;i++)
                        {
                            stringList[i] = "";
                        }
                        ResetPictureBoxList2(13);
                        btnDanh.Visible = true;
                        btnDanh.Enabled = true;
                    }
                    //Trường người chơi trước bỏ lượt, và lượt hiện tại của người chơi này
                    else if (result.Contains("turn") && result.Length == 5)
                    {
                        effect(pictureBox27, Myturn);
                        if (op[0] == Convert.ToInt16(result.Last().ToString()))
                        {
                            stringList[0] = result.Last().ToString() + "boluot";
                        }
                        if (op[1] == Convert.ToInt16(result.Last().ToString()))
                        {
                            stringList[1] = result.Last().ToString() + "boluot";
                        }
                        if (op[2] == Convert.ToInt16(result.Last().ToString()))
                        {
                            stringList[2] = result.Last().ToString() + "boluot";
                        }
                        for (int i = 0; i < players - 1; i++)
                        {
                            if (op[i] > -1)
                            {
                                resetEffect(pictureBoxList3[i], op[i]);
                            }
                        }
                        for (int i = 0; i < players-1; i++)
                        {
                            if (op[i] > -1 && stringList[i].Contains("boluot"))
                            {
                                effectMatLuot(pictureBoxList3[i], op[i]);
                            }
                        }
                        btnDanh.Visible = true;
                        btnBoLuot.Visible = true;
                        btnDanh.Enabled = true;
                        btnBoLuot.Enabled = true;
                    }
                    //Trường hợp tới lượt đánh của người chơi
                    else if (result.Contains("turn"))
                    {
                        effect(pictureBox27,Myturn);
                        
                        for (int i = 0; i < players-1; i++)
                        {
                            if (op[i] == Convert.ToInt16(result.Last().ToString()))
                            {
                                stringList[i] = result.Last().ToString();
                            }
                        }
                        for (int i = 0; i < players - 1; i++)
                        {
                            if (op[i] > -1)
                            {
                                resetEffect(pictureBoxList3[i], op[i]);
                            }
                        }
                        for (int i = 0; i < players-1; i++)
                        {
                            if (op[i] > -1 && stringList[i].Contains("boluot"))
                            {
                                effectMatLuot(pictureBoxList3[i], op[i]);
                            }
                        }
                        result = result.Remove(result.Length - 5);//Xóa 4 kí tự cuối chuối(là string "turn")
                        btnDanh.Enabled = true;
                        btnBoLuot.Enabled = true;
                        ConvertToListDouble(BaiCuaDoiThu, result);
                        string temp = ConvertToString(BaiCuaDoiThu);
                        //txtBaiCuaDoiThu.Text = temp;
                        HienThiBai(temp);

                    }
                    //Đây là trường hợp đã có người chơi trong bàn win. Tiến hành Load bài đó và set lại game
                    else if (result.Contains("win"))
                    {
                        MessageBox.Show("Player " + result.Last() + " win");
                        result = result.Remove(result.Length - 4);
                        ConvertToListDouble(BaiCuaDoiThu, result);
                        /*txtBaiCuaDoiThu.Text = ConvertToString(BaiCuaDoiThu);*///Load bài của đối thủ

                        for (int i = 0; i < players-1; i++)
                        {
                            if (op[i] > -1)
                            {
                                resetEffect(pictureBoxList3[i], op[i]);
                            }
                        }

                        for(int i=0;i<players-1;i++)
                        {
                            stringList[i] = "";
                        }
                        BaiHienTai.Clear();
                        BaiCuaDoiThu.Clear();
                        btnDanh.Enabled = false;
                        btnBoLuot.Enabled = false;
                        btnReady.Enabled = true;
                    }
                    //Đây là trường hợp người chơi không có lượt đánh. Chỉ nhận được bài của đối thủ
                    else if (char.IsDigit(result[0]))
                    {
                        
                        
                        if (op[0] == Convert.ToInt16(result.Last().ToString()))
                        {
                            stringList[0] = result.Last().ToString();
                            resetEffect(pictureBoxList3[0], op[0]);
                            for (int i = 1; i < players-1; i++)
                            {
                                if (op[i] > -1 && !stringList[i].Contains("boluot"))
                                {
                                    effect(pictureBoxList3[i],op[i]);
                                    break;
                                }
                            }
                        }
                        if (op[1] == Convert.ToInt16(result.Last().ToString()))
                        {
                            stringList[1] = result.Last().ToString();
                            resetEffect(pictureBoxList3[1], op[1]);
                            if (op[2] > -1 && !stringList[2].Contains("boluot"))
                                effect(pictureBoxList3[2],op[2]);
                            else
                            {
                                if (op[0] > -1 && !stringList[0].Contains("boluot"))
                                    effect(pictureBoxList3[0],op[0]);
                            }
                        }
                        if (op[2] == Convert.ToInt16(result.Last().ToString()))
                        {
                            stringList[2] = result.Last().ToString();
                            resetEffect(pictureBoxList3[2], op[2]);
                            for(int i=0;i<players-1;i++)
                            {
                                if(op[i]>-1&&!stringList[i].Contains("boluot"))
                                {
                                    effect(pictureBoxList3[i],op[i]);
                                    break;
                                }
                            }
                        }
                        for (int i = 0; i < players - 1; i++)
                        {
                            if (op[i] > -1 && stringList[i].Contains("boluot"))
                            {
                                effectMatLuot(pictureBoxList3[i], op[i]);
                            }
                        }
                        result = result.Remove(result.Length - 1);
                        ConvertToListDouble(BaiCuaDoiThu, result);
                        /*txtBaiCuaDoiThu.Text = ConvertToString(BaiCuaDoiThu);*///Load bài của đối thủ
                        string temp = ConvertToString(BaiCuaDoiThu);
                        HienThiBai(temp);
                    }
                    else if (result.Contains("boluot"))
                    {
                        
                        //MessageBox.Show("Player " + result.Last() + " pass");
                        
                        if (op[0] == Convert.ToInt16(result.Last().ToString()))
                        {
                            effectMatLuot(pictureBox28, op[0]);
                            stringList[0] = result.Last().ToString()+"boluot";
                            for (int i = 1; i < players-1; i++)
                            {
                                if (op[i] > -1 && !stringList[i].Contains("boluot"))
                                {
                                    effect(pictureBoxList3[i], op[i]);
                                    break;
                                }
                            }
                        }
                        if (op[1] == Convert.ToInt16(result.Last().ToString()))
                        {
                            effectMatLuot(pictureBox29, op[1]);
                            stringList[1] = result.Last().ToString()+"boluot";
                            if (op[2] > -1 && !stringList[2].Contains("boluot"))
                                effect(pictureBoxList3[2], op[2]);
                            else
                            {
                                if (op[0] > -1 && !stringList[0].Contains("boluot"))
                                    effect(pictureBoxList3[0], op[0]);
                            }
                        }
                        if (op[2] == Convert.ToInt16(result.Last().ToString()))
                        {
                            effectMatLuot(pictureBox30, op[2]);
                            stringList[2] = result.Last().ToString()+"boluot";
                            for (int i = 0; i < players-1; i++)
                            {
                                if (op[i] > -1 && !stringList[i].Contains("boluot"))
                                {
                                    effect(pictureBoxList3[i], op[i]);
                                    break;
                                }
                            }
                        }
                        //for (int i = 0; i < 3; i++)
                        //{
                        //    if (op[i] == Convert.ToInt16(result.Last().ToString()))
                        //    {
                        //        textBoxList[i].Text = result.Last().ToString() + "boluot";
                        //    }
                        //}
                        for (int i = 0; i < players-1; i++)
                        {
                            if (op[i] > -1 && stringList[i].Contains("boluot"))
                            {
                                effectMatLuot(pictureBoxList3[i], op[i]);
                            }
                        }
                    }
                    else if(result.Contains("reset"))
                    {
                        if (op[0] == Convert.ToInt16(result.Last().ToString()))
                        {
                           
                            stringList[0] = result.Last().ToString() + "boluot";
                        }
                        if (op[1] == Convert.ToInt16(result.Last().ToString()))
                        {
                            
                            stringList[1] = result.Last().ToString() + "boluot";
                        }
                        if (op[2] == Convert.ToInt16(result.Last().ToString()))
                        {
                           
                            stringList[2] = result.Last().ToString() + "boluot";
                        }

                        for (int i = 0; i < players - 1; i++)
                        {
                            if (op[i] > -1 && stringList[i].Contains("boluot"))
                            {
                                effectMatLuot(pictureBoxList3[i], op[i]);
                            }
                        }

                        Thread.Sleep(1000);
                        resetEffect(pictureBox27,Myturn);
                        for (int i = 0; i < players-1; i++)
                        {
                            if (op[i] > -1)
                            {
                                resetEffect(pictureBoxList3[i], op[i]);
                            }
                        }

                        for (int i=0;i<players-1;i++)
                        {
                            if(op[i]==Convert.ToInt16(result[result.Length-2].ToString()))
                            {
                                effect(pictureBoxList3[i],op[i]);
                                break;
                            }
                        }

                        for(int i=0;i<players-1;i++)
                        {
                            stringList[i] = "";
                        }
                        ResetPictureBoxList2(13);
                    }
                    result = "";
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace);
                }
            }
        }
        private void btnBoLuot_Click(object sender, EventArgs e)
        {

            btnDanh.Enabled = false;
            tcpForPlayer.SendData("boluot");
            btnBoLuot.Enabled = false;
            effectMatLuot(pictureBox27,Myturn);

            for(int i=0;i<players-1;i++)
            {
                if(op[i]>-1&&stringList[i].Contains("boluot"))
                {
                    effectMatLuot(pictureBoxList3[i],op[i]);
                }
            }

            for (int i = 0; i < players-1; i++)
            {
                if (op[i] > -1 && !stringList[i].Contains("boluot"))
                {
                    effect(pictureBoxList3[i],op[i]);
                    break;
                }
            }
        }
        private void btnDanh_Click(object sender, EventArgs e)
        {
            Danhbai();
        }

        void Danhbai()
        {
            for (int i = 0; i < players - 1; i++)
            {
                if (op[i] > -1 && stringList[i].Contains("boluot"))
                {
                    effectMatLuot(pictureBoxList3[i], op[i]);
                }
            }
            List<double> Buffer = new List<double>();
            //Viết xử lý ở đây: 
            //Khi bấm vào 1 quân bài thì sẽ có 1 double được add về buffer
            string input = String_Fight.ToString();
            input = input.Trim();
            string[] Input = input.Split('\t');
            foreach (var i in Input)
                Buffer.Add(Convert.ToDouble(i));
            Buffer.Sort();
            string flag = "";
            //if (Buffer.Count == 0)
            //    return;
            if(signal==1&&Buffer[0]!=BaiHienTai[0])
            {

                if (Buffer.Count != 0)
                {
                    //foreach (var i in Buffer)
                    //    BaiHienTai.Remove(i);
                    for (int i = 0; i < Buffer.Count; i++)
                    {
                        double bienTam = Buffer[i];
                        for (int j = 0; j < 13; j++)
                        {
                            if (Convert.ToDouble(pictureBoxList1[j].Tag) == bienTam)
                            {
                                pictureBoxList1[j].Location = new Point(pictureBoxList1[j].Location.X, pictureBoxList1[j].Location.Y + 20);
                                //Buffer.Remove(Buffer[i]);
                                //--i;
                                //break;
                            }
                        }
                    }
                    Buffer.Clear();
                    String_Fight.Remove(1);
                    String_Fight = "";
                    return;
                }
            }
            if (XuLyBai.KiemTraHopLe(Buffer, BaiCuaDoiThu) == true)//Nếu những quân bài đánh ra hợp lệ thì làm 3 viêc: 1. Xóa những quân bài đã đánh ra khỏi List BaiHienTai; 2.Nếu số quân bài còn lại ==0 thì gán thêm flag win cho server; 3. Trả về kết quả đánh
            {
                if (signal == 1)
                    signal = 0;
                resetEffect(pictureBox27,Myturn);
                string temp;
                temp = ConvertToString(Buffer);
                HienThiBai(temp);

                foreach (var i in Buffer)
                    BaiHienTai.Remove(i);
                BaiHienTai.Sort();

                for (int i = 0; i < 13; i++)
                {
                    pictureBoxList1[i].Image = null;
                    pictureBoxList1[i].BackColor = this.BackColor;
                    pictureBoxList1[i].BorderStyle = BorderStyle.None;
                    pictureBoxList1[i].Location = new Point(pictureBoxList1[i].Location.X, Y);
                    pictureBoxList1[i].Tag = null;
                    //txtDanh.Clear();
                }

                for (int i = BaiHienTai.Count(); i < 13; i++)
                {
                    pictureBoxList1[i].Visible = false;
                }

                for (int i = 0; i < BaiHienTai.Count(); i++)
                {

                    pictureBoxList1[i].Image = Image.FromFile(path + BaiHienTai[i].ToString() + ".png");
                    pictureBoxList1[i].BackColor = Color.White;
                    pictureBoxList1[i].BorderStyle = BorderStyle.FixedSingle;
                    pictureBoxList1[i].Tag = BaiHienTai[i];

                }
                //Gọi hàm load hình LoadHinh(Buffer)

                /*txbBai.Text = ConvertToString(BaiHienTai);*/// gọi hàm load hinh; LoadHinh(BaiHienTai)
                if (BaiHienTai.Count == 0)
                {
                    flag = "win";
                    BaiCuaDoiThu.Clear();
                    for (int i = 0; i < players-1; i++)
                    {
                        if (op[i] > -1)
                        {
                            resetEffect(pictureBoxList3[i], op[i]);
                        }
                    }
                    btnReady.Enabled = true;
                    tcpForPlayer.SendData(ConvertToString(Buffer) + flag);
                    Buffer.Clear();
                    String_Fight.Remove(1);
                    String_Fight = "";
                    btnDanh.Enabled = false;
                    btnBoLuot.Enabled = false;
                    return;
                }
                tcpForPlayer.SendData(ConvertToString(Buffer) + flag);
                Buffer.Clear();
                String_Fight.Remove(1);
                String_Fight = "";
                btnDanh.Enabled = false;
                btnBoLuot.Enabled = false;
                for (int i=0;i<players-1;i++)
                {
                    if(op[i]>-1&&!stringList[i].Contains("boluot"))
                    {
                        effect(pictureBoxList3[i],op[i]);
                        break;
                    }
                }
            }
            else
            {
                if (Buffer.Count != 0)
                {
                    //foreach (var i in Buffer)
                    //    BaiHienTai.Remove(i);
                    for (int i = 0; i < Buffer.Count; i++)
                    {
                        double bienTam = Buffer[i];
                        for (int j = 0; j < 13; j++)
                        {
                            if (Convert.ToDouble(pictureBoxList1[j].Tag) == bienTam)
                            {
                                pictureBoxList1[j].Location = new Point(pictureBoxList1[j].Location.X, pictureBoxList1[j].Location.Y + 20);
                                //Buffer.Remove(Buffer[i]);
                                //--i;
                                //break;
                            }
                        }
                    }
                    Buffer.Clear();
                    String_Fight.Remove(1);
                    String_Fight = "";
                }
            }
        }

        private void fClick_pBox1(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Location.Y == Y)
            {
                ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X, ((PictureBox)sender).Location.Y - 20);
                String_Fight += ((PictureBox)sender).Tag.ToString() + '\t';
                return;

            }
            if (((PictureBox)sender).Location.Y < Y)
            {
                string delete= ((PictureBox)sender).Tag.ToString();
                //int lengthLaBai = ((PictureBox)sender).Tag.ToString().Count() + 1;
                ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X, ((PictureBox)sender).Location.Y + 20);
                //String_Fight = String_Fight.Remove(String_Fight.Length - lengthLaBai);
                List<double> temp = new List<double>();
                String_Fight = String_Fight.Trim();
                string[] Result = String_Fight.Split('\t');
                for (int i = 0; i < Result.Length; i++)
                    temp.Add(Convert.ToDouble(Result[i]));
               
                for(int i=0;i<temp.Count;i++)
                    if (temp[i] == Convert.ToDouble(delete))
                        temp.Remove(temp[i]);
                String_Fight = ConvertToString(temp);
                return;
            }
        }

        void ConvertToListDouble(List<double> Cards, string String)
        {
            BaiCuaDoiThu.Clear();
            String = String.Trim();
            string[] Result = String.Split('\t');
            for (int i = 0; i < Result.Length; i++)
            {
                Cards.Add(Convert.ToDouble(Result[i]));
            }
            Cards.Sort();
        }
        string ConvertToString(List<double> Cards)
        {
            string String = "";
            foreach (var i in Cards)
                String += i.ToString() + "\t";
            return String;

        }
        public void HienThiBai(string b)
        {
            b = b.Trim();
            string[] a = b.Split('\t');
            ResetPictureBoxList2(13);

            for (int i = 0; i < a.Length; i++)
            {
                pictureBoxList2[i].BringToFront();
                pictureBoxList2[i].Tag = a[i];
                pictureBoxList2[i].BackColor = Color.White;
                pictureBoxList2[i].BackgroundImage = Image.FromFile(path + a[i] + ".png");
                pictureBoxList2[i].Visible = true;
            }

        }

        private void buttonDanh_MouseHover(object sender, EventArgs e)
        {
            btnDanh.FlatAppearance.BorderSize = 2;
            btnDanh.FlatAppearance.BorderColor = Color.BlueViolet;
        }

        private void buttonDanh_MouseLeave(object sender, EventArgs e)
        {
            btnDanh.FlatAppearance.BorderSize = 2;
            btnDanh.FlatAppearance.BorderColor = Color.Black;
        }

        private void buttonBoLuot_MouseHover(object sender, EventArgs e)
        {
            btnBoLuot.FlatAppearance.BorderSize = 2;
            btnBoLuot.FlatAppearance.BorderColor = Color.BlueViolet;
        }

        private void buttonBoLuot_MouseLeave(object sender, EventArgs e)
        {
            btnBoLuot.FlatAppearance.BorderSize = 2;
            btnBoLuot.FlatAppearance.BorderColor = Color.Black;
        }
    }
}
