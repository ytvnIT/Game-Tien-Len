using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class XuLyBai
    {
        /*========================KIỂM TRA TỚI TRẮNG =========================*/
        //Có 3 trường hợp cho bài tới trắng: Tứ quý heo, Sảnh 2->Át, 6 đôi
        public static bool KiemTraToiTrang(List<double> BaiHienTai)
        {
            if (KiemTraTuQuy2(BaiHienTai) || KiemTraTu3ToiAt(BaiHienTai) || KiemTra6Doi(BaiHienTai))
                return true;
            return false;
        }
        static private bool KiemTraTuQuy2(List<double> BaiHienTai)
        {
            if (Convert.ToInt32(BaiHienTai[12]) != 15)
                return false;
            for (int i = 9; i < 12; i++)
                if (Convert.ToInt32(BaiHienTai[i + 1]) - Convert.ToInt32(BaiHienTai[i]) != 0)
                    return false;
            return true;
        }
        static private bool KiemTraTu3ToiAt(List<double> BaiHienTai)
        {
            if (Convert.ToInt16(BaiHienTai[0]) != 3)
                return false;
            int count = 0;
            for (int i = 0; i < 12; i++)
                if (Convert.ToInt32(BaiHienTai[i + 1]) - Convert.ToInt32(BaiHienTai[i]) == 1)
                    count++;
            if (count >= 11)
                return true;
            return false;
        }
        static private bool KiemTra6Doi(List<double> BaiHienTai)
        {
            int count = 0;
            int step = 0;
            for (int i = 0; i < 12; i = i + step)
                if (Convert.ToInt32(BaiHienTai[i + 1]) - Convert.ToInt32(BaiHienTai[i]) == 0)
                {
                    count++;
                    step = 2;
                }
                else step = 1;

            if (count == 6)
                return true;
            return false;
        }

        /*======================PHÂN LOẠI BÀI NHẬN ĐƯỢC===============================*/
        //Trả về tên loại bài
        public static string PhanLoaiBai(List<double> Buffer)
        {
            int count = Buffer.Count;
            if (count == 1)
                return "Coc";
            if (count == 2 && KiemTraBoi(Buffer))
                return "Doi";
            if (count == 3 || count == 4)
                if (KiemTraBoi(Buffer))
                    return "Boi";
                else if (KiemTraSanh(Buffer))
                    return "Sanh";
            if ((count == 5 || count == 7 || count == 9 || count == 11) && KiemTraSanh(Buffer))
                return "Sanh";
            if (count == 6 || count == 8 || count == 10)
                if (KiemTraDoiThong(Buffer) == true)
                    return "Doithong";
                else if (KiemTraSanh(Buffer) == true)
                    return "Sanh";
            return "Khac";
        }
        static private bool KiemTraBoi(List<double> Cards)// 2 la hoac 3 la hay 4 la giong nhau
        {
            for (int i = 0; i < Cards.Count - 1; i++)
                if (Convert.ToInt32(Cards[i]) != Convert.ToInt32(Cards[i + 1]))
                    return false;
            return true;
        }
        static private bool KiemTraSanh(List<double> Cards)
        {
            if (Cards.Contains(15.1) || Cards.Contains(15.2) || Cards.Contains(15.3) || Cards.Contains(15.4))//Nếu tồn tại con heo thì đó không phải là sảnh
                return false;
            for (int i = 0; i < Cards.Count - 1; i++)
                if (Convert.ToInt32(Cards[i + 1]) - Convert.ToInt32(Cards[i]) != 1)
                    return false;
            return true;
        }
        static private bool KiemTraDoiThong(List<double> Cards)
        {
            int count = Cards.Count;
            if (Cards.Contains(15.1) || Cards.Contains(15.2) || Cards.Contains(15.3) || Cards.Contains(15.4))
                return false;
            int j;
            for (int i = 0; i <= count - 2; i = i + 2)
            {
                j = i + 1;
                if (i == count - 2)
                {
                    if (Convert.ToInt32(Cards[j]) - Convert.ToInt32(Cards[i]) != 0)
                        return false;
                }
                else if (Convert.ToInt32(Cards[j]) - Convert.ToInt32(Cards[i]) != 0 || Convert.ToInt32(Cards[i + 2]) - Convert.ToInt32(Cards[j]) != 1)
                    return false;
            }
            return true;
        }

        /*==================KIỂM TRA HỢP LỆ==================*/
        //Mô tả: Kiểm tra những lá bài người chơi chuẩn bị đánh có phù hợp với lá bài của đối thủ đã đánh trước đó hay không

        public static bool KiemTraHopLe(List<double> BaiCuaNguoiChoi, List<double> BaiCuaDoiThu)
        {
            /*=======================LUẬT ĐÁNH BÌNH THƯỜNG==============*/
            //Nếu người chơi cầm cái thì người chơi được đánh quân bất kì nhưng phải theo luật
            if (BaiCuaDoiThu.Count == 0 && PhanLoaiBai(BaiCuaNguoiChoi) != "Khac")
                return true;
            //Bài đánh hợp lệ nếu số quân bài bằng nhau, quân bài cùng loại bài, con lớn nhất của người chơi phải lớn hơn đối thủ
            if (BaiCuaNguoiChoi.Count == BaiCuaDoiThu.Count && PhanLoaiBai(BaiCuaNguoiChoi) == PhanLoaiBai(BaiCuaDoiThu) && BaiCuaNguoiChoi.Last()>BaiCuaDoiThu.Last())
                return true;


            /*=========================LUẬT CHẶT======================*/
            //Bài của người chơi là tứ quý thì có thể chặt heo của đối phương
            if ( (BaiCuaDoiThu.Count() == 1 && Convert.ToInt16(BaiCuaDoiThu[0]) == 15) && ( BaiCuaNguoiChoi.Count() == 4 && PhanLoaiBai(BaiCuaNguoiChoi) == "Boi"))
                return true;
            //Bài của người chơi là ba đôi thông thì có thể chặt heo của đối phương
            if ((BaiCuaDoiThu.Count() == 1 && Convert.ToInt16(BaiCuaDoiThu[0]) == 15) && (BaiCuaNguoiChoi.Count() == 6 && PhanLoaiBai(BaiCuaNguoiChoi) == "Doithong"))
                return true;
            //Nếu bài của người chơi là bốn đôi thông
            if ((BaiCuaNguoiChoi.Count() == 8 && PhanLoaiBai(BaiCuaNguoiChoi) == "Doithong"))
            {
                //Nếu bài của đối thử là 1 hoặc 2 cây hai
                if( (BaiCuaDoiThu.Count() == 1 && Convert.ToInt16(BaiCuaDoiThu[0]) == 15) || (PhanLoaiBai(BaiCuaDoiThu)=="Doi"&&Convert.ToInt16(BaiCuaDoiThu[0])==15) )
                    return true;
                //Nếu bài của đối thủ là ba đôi thông
                if (BaiCuaNguoiChoi.Count() == 6 && PhanLoaiBai(BaiCuaNguoiChoi) == "Doithong")
                    return true;
                //Nếu bài của đối thủ là tứ quý
                if (BaiCuaNguoiChoi.Count() == 4 && PhanLoaiBai(BaiCuaNguoiChoi) == "Boi")
                    return true;

            }
         return false;
        }

       

       

       
    }
}
