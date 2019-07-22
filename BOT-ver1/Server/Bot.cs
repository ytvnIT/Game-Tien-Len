using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    class Bot
    {


        private List<double> BaiHienTai;
        public List<double> BaiCuaDoiThu;
        public string sBaiHienTai;
        public string sBaiCuaDoiThu;
        List<double> Buffer = new List<double>();

        public Bot()
        {
            sBaiCuaDoiThu = "";
            sBaiHienTai = "";
            BaiHienTai   = new List<double>();
            BaiCuaDoiThu = new List<double>();
        }

        //private void btnReady_Click(object sender, EventArgs e)
        //{
        //    tcpForPlayer.SendData("chiabai");
        //    string result = tcpForPlayer.ReadData();

        //    //Nếu người chơi nhận được lượt đánh thì enable btbDanh
        //    if (result.Contains("turn"))
        //    {
        //        result = result.Remove(result.Length - 4);//Xóa 4 kí tự cuối chuối(là string "turn")
        //        btnDanh.Enabled = true;
        //    }

        //    ConvertToListDouble(BaiHienTai, result);//Convert từ string sang List<double>
        //    //Kiểm tra xem bài của người cho có tới trắng hay không
        //    if (XuLyBai.KiemTraToiTrang(BaiHienTai) == true)
        //        tcpForPlayer.SendData(ConvertToString(BaiHienTai) + "wintrang");

        //    txbBai.Text = ConvertToString(BaiHienTai);//gọi hàm load hình LoadHinh(BaiHienTai)          
        //    btnReady.Enabled = false;
        //}
        public void NhanBai()
        {
            if (sBaiHienTai.Contains("turn"))
                sBaiHienTai = sBaiHienTai.Remove(sBaiHienTai.Length - 4);
            ConvertToListDouble(BaiHienTai, sBaiHienTai);
            string tmp = "";
            foreach (var i in BaiHienTai)
                tmp += i + "\t";
            MessageBox.Show(tmp);
        }

        public string Danh()//Dùng để nhận bài của đối thủ
        {
          
            
            //Nếu nhận được "newturn" thì người chơi có quyền đánh những lá tùy ý.
            if (sBaiCuaDoiThu == "newturn")
            {
                BaiCuaDoiThu.Clear();
                //Gọi hàm đánh
                string result = XuLyBaiServer.DanhChuDong(BaiHienTai);
                List<double> tmp = new List<double>();
                ConvertToListDouble(tmp, result);
                foreach (var i in tmp)
                    if (BaiHienTai.Contains(i))
                        BaiHienTai.Remove(i);
                if (BaiHienTai.Count == 0)
                    return result + "win";
                return result;
               
            }
            //Trường người chơi trước bỏ lượt, và lượt hiện tại của người chơi này
            else if (sBaiCuaDoiThu == "turn")
            {
                return TimBuffer();
            }
            //Trường hợp tới lượt đánh của người chơi
            else if (sBaiCuaDoiThu.Contains("turn"))
            {
                sBaiCuaDoiThu = sBaiCuaDoiThu.Remove(sBaiCuaDoiThu.Length - 4);
                ConvertToListDouble(BaiCuaDoiThu, sBaiCuaDoiThu);
                return TimBuffer();
                //Xóa 4 kí tự cuối chuối(là string "turn")
                                                                               //Xử lý input, => BaiCuaDoiThu => Gọi hàm đánh

            }
            //Đây là trường hợp đã có người chơi trong bàn win. Tiến hành Load bài đó và set lại game
            else if (sBaiCuaDoiThu.Contains("win"))
            {
                sBaiHienTai="";
                sBaiCuaDoiThu="";
                BaiHienTai.Clear();
                BaiCuaDoiThu.Clear();
                

            }
            //Đây là trường hợp người chơi không có lượt đánh. Chỉ nhận được bài của đối thủ
            else if (char.IsDigit(sBaiCuaDoiThu[0]))
            {
                ConvertToListDouble(BaiCuaDoiThu, sBaiCuaDoiThu);

            }
            return "";
        }

        public void ResetBot()
        {
            sBaiHienTai = "";
            sBaiCuaDoiThu = "";
            BaiHienTai.Clear();
            BaiCuaDoiThu.Clear();
        }
        string Danhbai()
        {
            return "3.1";
            //Viết xử lý ở đây: 
            //Khi bấm vào 1 quân bài thì sẽ có 1 double được add về buffer
            TimBuffer();
            string flag = "";
            if (XuLyBaiServer.KiemTraHopLe(Buffer, BaiCuaDoiThu) == true)//Nếu những quân bài đánh ra hợp lệ thì làm 3 viêc: 1. Xóa những quân bài đã đánh ra khỏi List BaiHienTai; 2.Nếu số quân bài còn lại ==0 thì gán thêm flag win cho server; 3. Trả về kết quả đánh
            {
                //Gọi hàm load hình LoadHinh(Buffer)
                foreach (var i in Buffer)
                    BaiHienTai.Remove(i);

                if (BaiHienTai.Count == 0)
                {
                    flag = "win";
                    BaiCuaDoiThu.Clear();

                }

                Buffer.Clear();
                return ConvertToString(Buffer) + flag;


            }
        }

        string TimBuffer()
        {
            string LoaiBai = XuLyBaiServer.PhanLoaiBai(BaiCuaDoiThu);
            string result = "";

            switch(LoaiBai)
            {
                case "Coc":
                    result = XuLyBaiServer.DanhCoc(BaiCuaDoiThu.Last(), BaiHienTai);
                    break;
                case "Doi":
                    result = XuLyBaiServer.DanhBoi(BaiCuaDoiThu.Last(), BaiCuaDoiThu.Count() - 1, BaiHienTai);
                    break;
                case "Boi":// Trường hợp Sam(SL=2) và Tứ quý(SL=3) 
                    result = XuLyBaiServer.DanhBoi(BaiCuaDoiThu.Last(), BaiCuaDoiThu.Count() - 1, BaiHienTai);
                    break;             
                case "Sanh":
                    result = XuLyBaiServer.DanhSanh(BaiCuaDoiThu.Last(), BaiCuaDoiThu.Count(), BaiHienTai);
                    break;
                case "DoiThong":
                    result = XuLyBaiServer.DanhBoi(BaiCuaDoiThu.Last(), BaiCuaDoiThu.Count() / 2, BaiHienTai);
                    break;
            }
            if (result!="")
            {
                List<double> tmp = new List<double>();
                ConvertToListDouble(tmp, result);
                foreach (var i in tmp)
                    if (BaiHienTai.Contains(i))
                        BaiHienTai.Remove(i);
                if (BaiHienTai.Count == 0)
                    return result + "win";
                return result;
            }
                
            return "boluot";
        }

      
        public void ConvertToListDouble(List<double> Cards, string String)
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


    }
}
