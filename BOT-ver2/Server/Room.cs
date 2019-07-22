using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    class Room
    {
        public List<Player> players;
        private int nguoiDangThang;
        public int soTienCuoc;
        public bool isPlaying;
        public int soNguoiTrongPhong;
        public int readyPlayers;
        public int turn;// Khi turn=1 nghĩa là lượt đánh của người chơi 1; turn=2 nghĩa là lượt của người 2...
        public List <int> DanhSachBoLuot;// Người chơi này bỏ lượt thì index của người đó sẽ được lưu vào đây
        private int sovan;// số ván bài đã chơi trong phòng
        private BoBai bobai;
        public int soNguoiChoiTaiLucChiaBai;
        public List<Bot> AI;

        public Room()
        {           
            players = new List<Player>(4);
             AI = new List<Bot>();
            nguoiDangThang = 0;
            soTienCuoc = 1000;
            isPlaying = false;
            soNguoiTrongPhong = 0;
            readyPlayers = 0;
            bobai = new BoBai();
            sovan = 0;
            DanhSachBoLuot = new List<int>();
            isPlaying = false;
        }

        public void ResetRoom(int playerWin)
        {
           
            nguoiDangThang = playerWin;            
            readyPlayers = 0;
            bobai = new BoBai();
            DanhSachBoLuot = new List<int>();
            turn = playerWin;
            sovan += 1;          
            isPlaying = false;
            AI.Clear();
        }

        //public int Ready()
        //{
        //    readyPlayers++;
        //    return readyPlayers;
        //}
        
        public void chiaBai(SocketModel[] SocketList)
        {
            //Khi chia bài thì đưa ra tín hiệu là phòng đang chơi và set role của người chơi là 1
            
            isPlaying = true;
            soNguoiChoiTaiLucChiaBai = players.Count();
            
            for (int i = 0; i < 4 - soNguoiChoiTaiLucChiaBai; i++)
                    AI.Add(new Bot());

            bobai.xaoBai();         
            int count = players.Count()+AI.Count(); // Số người chơi hiện tại trong phòng
            string[] NguoiChoi = new string[4];//Giá trị bài mà mỗi người chơi nhận được sẽ được lưu tạm vào đây, Sau khi đủ 13 lá sẽ được gửi về client
            int indexOfPlayer;// chỉ số của List Player chạy từ 0 tới count (max(count)=4) 
            for (int i = 0; i < count * 13; i = indexOfPlayer + i)
            {
                indexOfPlayer = 0;
                for (int k = nguoiDangThang; k < count; k++){                  
                    if (i + indexOfPlayer < count * 13)
                        NguoiChoi[k] += bobai.boBai[i + indexOfPlayer].LayBai() + "\r\t";
                    indexOfPlayer++;
                }
                nguoiDangThang = 0;
            }
            
            //Nếu là ván đầu tiên, thì lượt đánh dành cho người có cầm lá nhỏ nhất
            if (sovan == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if(i<players.Count)
                    {
                        if (NguoiChoiCoQuanBaiNhoNhat(NguoiChoi) == i)
                        {
                            turn = i;
                            SocketList[players[i].pos].SendData(NguoiChoi[i] + "turn");
                        }
                        else
                            SocketList[players[i].pos].SendData(NguoiChoi[i]);
                    }
                    else
                    {
                        if (NguoiChoiCoQuanBaiNhoNhat(NguoiChoi) == i)
                        {
                            turn = i;
                            AI[i-players.Count].sBaiHienTai = NguoiChoi[i] + "turn";
                            AI[i - players.Count].NhanBai();
                            AI[i - players.Count].Danh();
                        }
                        else
                        {
                            AI[i - players.Count].sBaiHienTai = NguoiChoi[i];
                            AI[i - players.Count].NhanBai();
                        }
                    }

                   
                }
            }
            //Nếu là ván thứ hai trở đi, thì lượt đánh dành cho người chơi thắng ván trước đó
            else
            {
                if(turn<players.Count)
                {
                    SocketList[players[turn].pos].SendData(NguoiChoi[turn] + "turn");                  
                }
                else
                {
                    AI[turn - players.Count].sBaiHienTai = NguoiChoi[turn] + "turn";
                    AI[turn - players.Count].NhanBai();
                    AI[turn - players.Count].Danh();
                }
                for(int i=0;i<players.Count;i++)
                    if(i!=turn)
                        SocketList[players[i].pos].SendData(NguoiChoi[i]);
                for (int i = 0; i < AI.Count; i++)
                    if (i != turn - players.Count)
                    {
                        AI[i].sBaiHienTai = NguoiChoi[i+players.Count];
                        AI[i].NhanBai();
                    }


            }
            
        }


        void ConvertToListDouble(List<double> Cards, string String)
        {
            
            String = String.Trim();
            string[] Result = String.Split('\t');
            for (int i = 0; i < Result.Length; i++)
            {
                Cards.Add(Convert.ToDouble(Result[i]));
            }
            Cards.Sort();
        }
        int NguoiChoiCoQuanBaiNhoNhat(string [] NguoiChoi)
        {
            Double min = 15.4; int index = 0;
            for (int i = 0; i < players.Count(); i++)
            {
                List<double> dNguoiChoi = new List<double>();
                ConvertToListDouble(dNguoiChoi, NguoiChoi[i]);
                if (dNguoiChoi.Min() < min)
                {
                    min = dNguoiChoi.Min();
                    index = i;
                }
                dNguoiChoi.Clear();
            }
            return index;
        }
       
    }  
}
