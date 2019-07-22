using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Player
    {
        string userName;
        string avatar;
        public int soTienConLai;
        public int pos;
        int soQuanBaiConLai;
        public int room;
        public Player(int index)
        {
            //userName = name;
            //avatar = "";
            soTienConLai = 10000;
            soQuanBaiConLai = 13;
            pos = index;
            room = -1;

        }
    }
}
