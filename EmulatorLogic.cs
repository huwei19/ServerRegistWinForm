using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRegistWinForm
{
    class EmulatorLogic
    {
        public static Dictionary<int, ServerListData> s_dicServerList = new Dictionary<int, ServerListData>();
        public class ServerListData
        {
            public int serverID;
            public string ip;
            public int port;
            public int regionID;
        }
    }
}
