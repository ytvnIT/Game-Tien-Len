using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {
        private TCPModel tcp;
        private SocketModel[] socketList1;
        private SocketModel[] socketList2;
        private int numberOfPlayers = 200;
        private int currentClient;
        private object thislock;

        List<Room> danhSachPhong;
        List<Player> danhSachNguoiChoi = new List<Player>();
        public Server()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            thislock = new object();
            danhSachPhong = new List<Room>(10);
            //  for (int i = 0; i < 10; i++)
            danhSachPhong.Add(new Room());
        }

        public void StartServer()
        {
            string ip = txbIP.Text;
            int port = int.Parse(txbPort.Text);
            tcp = new TCPModel(ip, port);
            tcp.Listen();
            btnStart.Enabled = false;
        }
        public void ServeClients()
        {
            socketList1 = new SocketModel[numberOfPlayers];
            socketList2 = new SocketModel[numberOfPlayers];
            for (int i = 0; i < numberOfPlayers; i++)
            {
                ServeAClient();
            }
        }
        public void Accept1()
        {
            int status = -1;
            Socket s = tcp.SetUpANewConnection(ref status);
            socketList1[currentClient] = new SocketModel(s);

            string str = socketList1[currentClient].GetRemoteEndpoint();
            txbConnectionManager.AppendText("\nNew connection from: " + str + "id" + currentClient + "\n");

        }
        public void Accept2()
        {
            int status = -1;
            Socket s = tcp.SetUpANewConnection(ref status);
            socketList2[currentClient] = new SocketModel(s);
            string str = socketList2[currentClient].GetRemoteEndpoint();
            txbConnectionManager.AppendText("\nNew connection from: " + str + "id" + currentClient + "\n");
        }

        public void ServeAClient()
        {
            int num = -1;
            lock (thislock)
            {
                Accept1();
                Accept2();
                currentClient++;
                num = currentClient - 1;
            }
            Thread t = new Thread(Commmunication);
            t.Start(num);
        }

        void DangNhap(int index)
        {
            danhSachNguoiChoi.Add(new Player(index));
        }

        int TimPhong(int index)
        {
            int j = 0;
            foreach (var i in danhSachPhong)
            {
                if (i.soNguoiTrongPhong < 4)
                {
                    //Thêm người chơi đó vào phòng
                    i.players.Add(danhSachNguoiChoi[index]);
                    //Set thuộc tính phòng cho người chơi
                    i.players[i.players.Count - 1].room = j;
                    i.soNguoiTrongPhong++;
                    if (i.soNguoiTrongPhong == 4)
                        danhSachPhong.Add(new Room());
                    return j;
                }
                j++;
            }
            return -1;
        }

        void ChiaBai(int sophong)
        {
            if (danhSachPhong[sophong].readyPlayers == danhSachPhong[sophong].soNguoiTrongPhong)
                danhSachPhong[sophong].chiaBai(socketList1);
        }

        void SetBoLuot(int sophong)
        {

            int luotbo = danhSachPhong[sophong].turn;
            while (danhSachPhong[sophong].DanhSachBoLuot.Contains(danhSachPhong[sophong].turn))
                danhSachPhong[sophong].turn = (danhSachPhong[sophong].turn + 1) % danhSachPhong[sophong].soNguoiChoiTaiLucChiaBai;

        }

        public void Commmunication(object obj)
        {
            int pos = (Int32)obj;
            while (true)
            {
                string str = socketList1[pos].ReceiveData();
                // Nếu người chơi đã đăng nhập và đã vào phong thì set biến phòng cho dể sử dụng
                int sophong = -1;
                if (danhSachNguoiChoi.Count > pos && danhSachNguoiChoi[pos].room != -1)
                    sophong = danhSachNguoiChoi[pos].room;

                if (str == "dangnhap")
                {
                    DangNhap(pos);
                    socketList1[pos].SendData("Đăng nhập thành công!");
                }
                if (str == "timphong")
                {
                    int room = TimPhong(pos) + 1;
                    socketList1[pos].SendData("Bạn đã được thêm vào phòng số " + room);

                    if (danhSachPhong[danhSachNguoiChoi[pos].room].isPlaying == true)
                        socketList1[pos].SendData("isplaying");
                    else
                        socketList1[pos].SendData("notyetplaying");

                }
                if (str == "chiabai")
                {
                    danhSachPhong[danhSachNguoiChoi[pos].room].readyPlayers++;
                    ChiaBai(danhSachNguoiChoi[pos].room);
                }
                //Khi Server nhận bài đánh ra từ các người chơi, Server sẽ broadcast cho các người chơi còn lại
                if (char.IsDigit(str[0]) && !str.Contains("win"))
                {
                    //set turn
                    int oldTurn = danhSachPhong[sophong].turn;
                    danhSachPhong[sophong].turn = (danhSachPhong[sophong].turn + 1) % danhSachPhong[sophong].soNguoiChoiTaiLucChiaBai;
                    SetBoLuot(sophong);

                    int turn = danhSachPhong[sophong].turn;
                    //Gửi bài kèm theo lượt cho người chơi
                    socketList2[danhSachPhong[sophong].players[turn].pos].SendData(str + "turn" + oldTurn);
                    //Gửi cho người chơi còn lại trong phòng
                    for (int i = 0; i < danhSachPhong[sophong].soNguoiTrongPhong; i++)
                    {
                        //  if (danhSachPhong[sophong].players[i].pos != pos || danhSachPhong[sophong].players[i].pos != turn)
                        //if (danhSachPhong[sophong].players[i].pos != pos || i!= turn)
                        //    socketList2[danhSachPhong[sophong].players[i].pos].SendData(str);
                        if (danhSachPhong[sophong].players[i].pos == pos || i == turn)
                            continue;
                        socketList2[danhSachPhong[sophong].players[i].pos].SendData(str + oldTurn);
                    }
                }
                if (str == "boluot")
                {
                    int oldTurn = danhSachPhong[sophong].turn;
                    //Thêm ID của người chơi hiện tại về danh sách bỏ lượt
                    danhSachPhong[sophong].DanhSachBoLuot.Add(danhSachPhong[sophong].turn);

                    //Nếu tất cả các người chơi khác đã bỏ lượt thì set lượt mới
                    if (danhSachPhong[sophong].DanhSachBoLuot.Count() == danhSachPhong[sophong].soNguoiChoiTaiLucChiaBai - 1)
                    {
                        for (int i = 0; i < danhSachPhong[sophong].soNguoiChoiTaiLucChiaBai; i++)
                            if (!danhSachPhong[sophong].DanhSachBoLuot.Contains(i))
                                danhSachPhong[sophong].turn = i;

                        socketList2[danhSachPhong[sophong].players[danhSachPhong[sophong].turn].pos].SendData("newturn" + oldTurn);
                        for (int i = 0; i < danhSachPhong[sophong].soNguoiTrongPhong; i++)
                        {
                            //danhSachPhong[sophong].players[i].pos == pos || 
                            if (i == danhSachPhong[sophong].turn)
                                continue;
                            socketList2[danhSachPhong[sophong].players[i].pos].SendData("reset" + danhSachPhong[sophong].turn+ oldTurn);
                        }
                        danhSachPhong[sophong].DanhSachBoLuot.Clear();
                    }
                    else
                    {
                        danhSachPhong[sophong].turn = (danhSachPhong[sophong].turn + 1) % danhSachPhong[sophong].soNguoiChoiTaiLucChiaBai;
                        SetBoLuot(sophong);
                        socketList2[danhSachPhong[sophong].players[danhSachPhong[sophong].turn].pos].SendData("turn" + oldTurn);
                        for (int i = 0; i < danhSachPhong[sophong].soNguoiTrongPhong; i++)
                        {
                            if (danhSachPhong[sophong].players[i].pos == pos || i == danhSachPhong[sophong].turn)
                                continue;
                            socketList2[danhSachPhong[sophong].players[i].pos].SendData("boluot" + oldTurn);
                        }
                    }
                }
                if (str.Contains("win"))
                {
                    danhSachPhong[sophong].ResetRoom(danhSachPhong[sophong].turn);
                    for (int i = 0; i < danhSachPhong[sophong].soNguoiTrongPhong; i++)
                        if (danhSachPhong[sophong].players[i].pos != pos)
                            socketList2[danhSachPhong[sophong].players[i].pos].SendData(str + danhSachPhong[sophong].turn);
                }

            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartServer();
            Thread t = new Thread(ServeClients);
            t.Start();
        }
    }
}
