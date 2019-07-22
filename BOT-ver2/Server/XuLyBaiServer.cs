using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
        class XuLyBaiServer

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
                if (BaiCuaNguoiChoi.Count == BaiCuaDoiThu.Count && PhanLoaiBai(BaiCuaNguoiChoi) == PhanLoaiBai(BaiCuaDoiThu) && BaiCuaNguoiChoi.Last() > BaiCuaDoiThu.Last())
                    return true;


                /*=========================LUẬT CHẶT======================*/
                //Bài của người chơi là tứ quý thì có thể chặt heo của đối phương
                if ((BaiCuaDoiThu.Count() == 1 && Convert.ToInt16(BaiCuaDoiThu[0]) == 15) && (BaiCuaNguoiChoi.Count() == 4 && PhanLoaiBai(BaiCuaNguoiChoi) == "Boi"))
                    return true;
                //Bài của người chơi là ba đôi thông thì có thể chặt heo của đối phương
                if ((BaiCuaDoiThu.Count() == 1 && Convert.ToInt16(BaiCuaDoiThu[0]) == 15) && (BaiCuaNguoiChoi.Count() == 6 && PhanLoaiBai(BaiCuaNguoiChoi) == "Doithong"))
                    return true;
                //Nếu bài của người chơi là bốn đôi thông
                if ((BaiCuaNguoiChoi.Count() == 8 && PhanLoaiBai(BaiCuaNguoiChoi) == "Doithong"))
                {
                    //Nếu bài của đối thử là 1 hoặc 2 cây hai
                    if ((BaiCuaDoiThu.Count() == 1 && Convert.ToInt16(BaiCuaDoiThu[0]) == 15) || (PhanLoaiBai(BaiCuaDoiThu) == "Doi" && Convert.ToInt16(BaiCuaDoiThu[0]) == 15))
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
        /*====================Bot đánh================*/
        public static string DanhCoc(Double max, List<double> BaiHienTai)
        {
            if (Convert.ToInt16(max) == 15 && DanhDoiThong(0, 3, BaiHienTai) != "")
                return DanhDoiThong(0, 3, BaiHienTai);
            if (Convert.ToInt16(max) == 15 && DanhDoiThong(0, 4, BaiHienTai) != "")
                return DanhDoiThong(0, 4, BaiHienTai);
            if (Convert.ToInt16(max) == 15 && DanhBoi(0, 3, BaiHienTai) != "")
                return DanhBoi(0, 3, BaiHienTai);

            foreach (var i in BaiHienTai)
                if (i > max)
                    return i.ToString();
            return "";
        }

        public static string DanhBoi(double max, int SL, List<double> BaiHienTai)
        {
            string result = "";
            int count = 0;
            for (int i = 0; i < BaiHienTai.Count - 1; i++)
                if (Convert.ToInt32(BaiHienTai[i + 1]) - Convert.ToInt32(BaiHienTai[i]) != 0)
                {
                    count = 0; result = ""; continue;
                }
                else
                {
                    if (count >= 1)
                        result += BaiHienTai[i + 1] + "\t";
                    else
                        result += BaiHienTai[i] + "\t" + BaiHienTai[i + 1] + "\t";
                    count++;
                    if (count == SL && BaiHienTai[i + 1] > max)
                        return result;
                }
            return "";
        }

        public static string DanhSanh(double max, int SL, List<double> BaiHienTai)
        {
            string result;
            for (int i = 0; i < BaiHienTai.Count - 1; i++)
            {
                result = "";
                int count = 0;
                for (int j = i; j < BaiHienTai.Count() - 1; j++)
                {
                    if (Convert.ToInt16(BaiHienTai[j + 1]) - Convert.ToInt16(BaiHienTai[j]) == 0)
                        continue;
                    if (Convert.ToInt16(BaiHienTai[j + 1]) - Convert.ToInt16(BaiHienTai[j]) != 1)
                        break;
                    result += BaiHienTai[j] + "\t";
                    count++;
                    if (count == SL - 1)
                        if (BaiHienTai[j + 1] < max|| Convert.ToInt16(BaiHienTai[j + 1])==15)
                            break;
                        else
                            return result + BaiHienTai[j + 1];
                }
            }
            return "";
        }

        public static string DanhDoiThong(double max, int SL, List<double> BaiHienTai)
        {
            string result;
            int step = 1;
            for (int i = 0; i < BaiHienTai.Count - 1; i++)
            {
                result = "";
                int count = 0;
                for (int j = i; j <= BaiHienTai.Count - 2; j = j + step)
                {
                    if (j == BaiHienTai.Count - 2)
                        if (Convert.ToInt32(BaiHienTai[j + 1]) - Convert.ToInt32(BaiHienTai[j]) != 0)
                            break;
                        else goto Loop;
                    step = 1;
                    if (Convert.ToInt32(BaiHienTai[j + 1]) - Convert.ToInt32(BaiHienTai[j]) == 0 && Convert.ToInt32(BaiHienTai[j + 2]) - Convert.ToInt32(BaiHienTai[j + 1]) == 0)
                        continue;
                    if (Convert.ToInt32(BaiHienTai[j + 1]) - Convert.ToInt32(BaiHienTai[j]) != 0 || Convert.ToInt32(BaiHienTai[j + 2]) - Convert.ToInt32(BaiHienTai[j + 1]) != 1)
                        break;

                    Loop:
                    result += BaiHienTai[j] + "\t" + BaiHienTai[j + 1] + "\t";
                    count++;
                    step = 2;
                    if (count == SL)
                        if (BaiHienTai[j + 1] < max)
                            break;
                        else
                            return result.Trim();
                }
            }
            return "";
        }

        public static string DanhChuDong(List<double> BaiHienTai)
        {
            //Nếu trong bài có sam thì ưu tiên đánh trước
            if (DanhBoi(0, 2, BaiHienTai) != "")
                return DanhBoi(0, 2, BaiHienTai);
            //Trường hợp bài có đôi, ưu tiên đánh đôi nhỏ trước 
            if (DanhBoi(0, 1, BaiHienTai) != "")
                return DanhBoi(0, 1, BaiHienTai);
            //Trường hợp đánh sảnh, ưu tiên đánh sảnh dài nhất trước
            for (int i = BaiHienTai.Count; i >= 3; i--)
                if (DanhSanh(0, i, BaiHienTai) != "")
                    return DanhSanh(0, i, BaiHienTai);
            if (Convert.ToInt16(Convert.ToDouble(DanhCoc(0, BaiHienTai))) <= 7)
                return DanhCoc(0, BaiHienTai);
            return DanhCoc(0, BaiHienTai);
        }

        /*======================CƯỜNG HÓA ĐIỂM MẠNH====================*/
        public static string DanhTheoDiemManh(List<double> BaiHienTai)
        {
            //Nếu bài mạnh về sảnh thì gọi hàm đánh sảnh(đánh sảnh dài nhất trước)
            if(TimDiemManh(BaiHienTai)==1)
                for (int i = BaiHienTai.Count; i >= 3; i--)
                    if (DanhSanh(0, i, BaiHienTai) != "")
                        return DanhSanh(0, i, BaiHienTai);
            if(TimDiemManh(BaiHienTai)==2&& DanhBoi(0, 1, BaiHienTai) != "")
                return DanhBoi(0, 1, BaiHienTai);
            return (DanhCoc(0, RacDoi));         
        }

        public static int TimDiemManh(List<double> BaiHienTai)
        {
            int n4 = DemSoSanh(BaiHienTai);
            int n3 = DemSoDoi(RacSanh);
            int n1 = 0, n2 = 0, n5 = 0;
            //n1: là cốc ghẻ; n2: là cốc thường; n5: là quân mạnh
            foreach (var i in RacDoi)
                if (i <= 5.4)
                    n1++;
                else if (i <= 12.4)
                    n2++;
                else if (i >= 13.1)
                    n5++;
            if ((n5 >= (n1 + n2)) && n4 >= 3)
                return 1;
            if (n3 >= 4)
                return 2;
            return -1;
        }
        //Những quân bài mà không thể tạo thành sảnh thì tạm gọi là rác sảnh
        public static List<double> RacSanh = new List<double>();
        public static int DemSoSanh(List<double> BaiHienTai)
        {
            int step = 0;
            int countSS = 0;// đếm số sảnh
            for (int i = 0; i < BaiHienTai.Count - 1; i = step)
            {
                List<double> SanhTam = new List<double>();
                int countSL = 0;
                SanhTam.Add(BaiHienTai[i]);
                for (int j = i; j < BaiHienTai.Count() - 1; j++)
                {

                    if (Convert.ToInt16(BaiHienTai[j + 1]) - Convert.ToInt16(BaiHienTai[j]) == 0)
                    {
                        if (!RacSanh.Contains(BaiHienTai[j + 1]))
                            RacSanh.Add(BaiHienTai[j + 1]);
                        if (j == BaiHienTai.Count - 2)
                        {
                            if (countSL < 2)
                            {
                                if (!SanhTam.Contains(BaiHienTai[j]))
                                    SanhTam.Add(BaiHienTai[j]);
                                // SanhTam.Add(BaiHienTai[j + 1]);
                            }
                            else
                            {
                                countSS++;
                                SanhTam.Clear();
                            }
                            step = j + 1;
                        }
                        continue;
                    }
                    if (Convert.ToInt16(BaiHienTai[j + 1]) - Convert.ToInt16(BaiHienTai[j]) != 1)
                    {
                        if (countSL < 2)
                        {
                            if (!RacSanh.Contains(BaiHienTai[j]))
                                SanhTam.Add(BaiHienTai[j]);
                            if (j == BaiHienTai.Count - 2)
                                SanhTam.Add(BaiHienTai[j + 1]);
                        }
                        else
                        {
                            countSS++;
                            SanhTam.Clear();
                            if (j == BaiHienTai.Count - 2)
                                RacSanh.Add(BaiHienTai[j + 1]);
                        }
                        step = j + 1;
                        break;
                    }
                    if (j == i)
                    {
                        // SanhTam.Add(BaiHienTai[j]);
                        SanhTam.Add(BaiHienTai[j + 1]);
                    }
                    else if (!SanhTam.Contains(BaiHienTai[j + 1]))
                        SanhTam.Add(BaiHienTai[j + 1]);

                    countSL++;
                    if (j == BaiHienTai.Count - 2)
                    {
                        if (countSL < 2)
                        {
                            if (!SanhTam.Contains(BaiHienTai[j]))
                                SanhTam.Add(BaiHienTai[j]);
                            //if (!SanhTam.Contains(BaiHienTai[j+1]))
                            //   SanhTam.Add(BaiHienTai[j + 1]);
                        }
                        else
                        {
                            countSS++;
                            SanhTam.Clear();
                        }
                        step = j + 1;
                    }

                }
                RacSanh.InsertRange(RacSanh.Count, SanhTam);
                RacSanh.Sort();
                SanhTam.Clear();
            }
            //Trả về số quân bài tạo được thành sảnh
            return BaiHienTai.Count - RacSanh.Count;
        }
        //Những quân bài không thể tạo thành đôi thì tạm gọi là rác đôi
        public static List<double> RacDoi = new List<double>();
        public static int DemSoDoi(List<double> BaiHienTai)
        {
            int count = 0;
            int step = 1;
            for (int i = 0; i < BaiHienTai.Count; i += step){
                if (i == BaiHienTai.Count - 1)
                    RacDoi.Add(BaiHienTai[i]);
                else if (Convert.ToInt16(BaiHienTai[i + 1]) - Convert.ToInt16(BaiHienTai[i]) != 0){
                    if (!RacDoi.Contains(BaiHienTai[i]))
                        RacDoi.Add(BaiHienTai[i]);
                    step = 1;
                }
                else {
                    count++;
                    step = 2;
                }
            }
            //Trả về số quân tạo được thành đôi
            return count*2;
        }

        

    }
    }


