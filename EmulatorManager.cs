using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ServerRegistWinForm
{
    class EmulatorManager
    {
        #region 单例
        //构造函数设置为 private
        private EmulatorManager()
        {
        }
        private static EmulatorManager instance = new EmulatorManager();
        public static EmulatorManager GetInstance()
        {
            return instance;
        }
        #endregion

        #region 存储结构
        //服务器数据 
        private List<ServerInfo> list_ServerData = new List<ServerInfo>();
        //登陆服数据 
        private List<ServerInfo> list_LoginData = new List<ServerInfo>();
        //模拟器配置信息
        private EmulatorConfig emulatorConfig = new EmulatorConfig();


        public class ServerInfo
        {
            //服务器id
            public int serverID = 0;

            //服务器名称
            public string serverName = "";

            //当前注册人数
            public int registedNum = 0;

            //最大注册人数
            public int registMaxNum = 0;

            //当前在线人数
            public int onlineNum = 0;

            //最大在线人数
            public int onlineMaxNum = 0;

            //当前排队人数
            public int queueNum = 0;

            //最大排队人数
            public int queueMaxNum = 0;

            //服务器是否开放注册
            public bool isOpenReg = false;
        }

        public class EmulatorConfig
        {
            //config.txt的 id
            public int configID;

            //每秒新增注册人数
            public int addRegNum;

            //每秒单服登录数
            public int addLoginNum;

            //每秒单服登出数
            public int delLoginNum;

            //服务器数据同步频率(单位秒)
            public int syncTime;

            //单服排队上限人数
            public int queueMaxLimit;

            //登录确认延时(暂时无效，后续添加功能)
            public int loginConfirmDeLay;
        }
        #endregion


        #region 获取 服务器数据

        /// <summary>
        /// 根据serverid 获取服务器存储数据
        /// </summary>
        /// <param name="serverid"></param>
        /// <returns></returns>
        public ServerInfo GetServerDataById(int serverid)
        {
            if (serverid <= 0)
            {
                return null;
            }
            ServerInfo serverInfo = list_ServerData.Find(t => t.serverID == serverid);
            return serverInfo;
        }

        /// <summary>
        /// 根据serverid 获取登陆服存储数据
        /// </summary>
        /// <param name="serverid"></param>
        /// <returns></returns>
        public ServerInfo GetLoginDataById(int serverid)
        {
            if (serverid <= 0)
            {
                return null;
            }
            ServerInfo serverInfo = list_LoginData.Find(t => t.serverID == serverid);
            return serverInfo;
        }
        #endregion


        #region 配置文件初始化
        /// <summary>
        /// 利用 配置文件 初始化数据
        /// </summary>
        public void InitData()
        {
            //用 ServerConfigList.txt 初始化数据
            InitData_ServerConfig();

            //用 Config.txt 初始化数据
            InitData_Config();
        }

        /// <summary>
        /// 根据 ServerConfigList.txt 初始化数据
        /// 四个属性 服务器id、服务器名称、最大注册人数、最大在线人数
        /// </summary>
        public void InitData_ServerConfig()
        {
            list_ServerData.Clear();
            list_LoginData.Clear();

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource/ServerConfigList.txt");
            string fileData = File.ReadAllText(filePath);
            if (null == fileData)
            {
                GameLog.LogError("ServerConfigList.txt load error");
                return;
            }
            string[] lines = fileData.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string[] serverDataInfo = lines[i].Split('\t');
                if (serverDataInfo.Length < 4)
                {
                    continue;
                }
                int serverID = 0;
                int registMaxNum = 0;
                int onlineMaxNum = 0;
                if (int.TryParse(serverDataInfo[0], out serverID) || int.TryParse(serverDataInfo[30], out registMaxNum) || int.TryParse(serverDataInfo[28], out onlineMaxNum))
                {
                    continue;
                }
                if (serverID <= 0 || registMaxNum <= 0 || onlineMaxNum <= 0)
                {
                    continue;
                }
                ServerInfo newData = new ServerInfo();
                newData.serverID = serverID; //服务器id
                newData.serverName = serverDataInfo[2];  //服务器名称
                newData.registMaxNum = registMaxNum; //最大注册人数
                newData.onlineMaxNum = onlineMaxNum;//最大在线人数

                GameLog.LogFormat("加载完成ServerConfigList配置文件，其中 serverID:{1}, serverName:{2}, registMaxNum:{3}, onlineMaxNum:{4}",
                    newData.serverID, newData.serverName, newData.registMaxNum, newData.onlineMaxNum);

                //添加到 服务器 和 登陆服数据 列表中
                list_ServerData.Add(newData);
                list_LoginData.Add(newData);
            }
        }

        /// <summary>
        /// 根据 Config.txt 初始化模拟器数据
        /// </summary>
        public void InitData_Config()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource/Config.txt");
            string fileData = File.ReadAllText(filePath);
            if (null == fileData)
            {
                GameLog.LogError("Config.txt load error");
                return;
            }
            string[] lines = fileData.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string[] serverDataInfo = lines[i].Split('\t');
                if (serverDataInfo.Length < 4)
                {
                    continue;
                }

                int configID = 0;
                if (int.TryParse(serverDataInfo[0], out configID))
                {
                    continue;
                }
                if (configID <= 0)
                {
                    continue;
                }
                emulatorConfig.configID = configID; //config.txt的 id
                int.TryParse(serverDataInfo[1], out emulatorConfig.addRegNum);//每秒新增注册人数
                int.TryParse(serverDataInfo[2], out emulatorConfig.addLoginNum);//每秒单服登录数
                int.TryParse(serverDataInfo[3], out emulatorConfig.delLoginNum);//每秒单服登出数
                int.TryParse(serverDataInfo[4], out emulatorConfig.syncTime);//服务器数据同步频率(单位秒)
                int.TryParse(serverDataInfo[5], out emulatorConfig.queueMaxLimit);//单服排队上限人数
                int.TryParse(serverDataInfo[6], out emulatorConfig.loginConfirmDeLay);//登录确认延时(暂时无效，后续添加功能)

                //找到一个正常数据就可以跳出循环
                break;
            }
            GameLog.LogFormat("加载完成config配置文件，其中 configID:{1}, addRegNum:{2}, addLoginNum:{3}, delLoginNum:{4}, syncTime:{5}, queueMaxLimit:{6}, loginConfirmDeLay:{7}",
                        emulatorConfig.configID, emulatorConfig.addRegNum, emulatorConfig.addLoginNum, emulatorConfig.delLoginNum,
                        emulatorConfig.syncTime, emulatorConfig.queueMaxLimit, emulatorConfig.loginConfirmDeLay);
        }
        #endregion

        /// <summary>
        /// 每隔一定时间 需要同步 登录服和服务器的数据
        /// </summary>
        public void SyncData()
        {

        }


        /// <summary>
        /// 每秒执行
        /// </summary>
        public void OnSecond()
        {

        }




    }
}
