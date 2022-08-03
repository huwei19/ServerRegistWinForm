using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

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

        //服务器数据 
        private List<ServerInfo> list_ServerData = new List<ServerInfo>();
        //登陆服数据 
        private List<ServerInfo> list_LoginData = new List<ServerInfo>();
        //模拟器配置信息
        private EmulatorConfig emulatorConfig = new EmulatorConfig();

        //秒计数器 用来控制 每隔 SyncTime 秒，同步服务器和登陆服数据
        private int SecondCount = 0;

        //模拟器总开关
        private bool isOpenEmulator = false;

        #region 服务器数据

        /// <summary>
        /// 获取 全部服务器数据
        /// </summary>
        /// <returns></returns>
        public List<ServerInfo> GetServerDatas()
        {
            return list_ServerData;
        }

        /// <summary>
        /// 获取登陆服 存储的 服务器数据
        /// </summary>
        /// <returns></returns>
        public List<ServerInfo> GetLoginDatas()
        {
            return list_LoginData;
        }


        /// <summary>
        /// 根据serverid 获取服务器存储数据
        /// </summary>
        /// <param name="serverid"></param>
        /// <returns></returns>
        public ServerInfo GetServerDataByServerID(int serverid)
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
        public ServerInfo GetLoginDataByServerID(int serverid)
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
                if (!int.TryParse(serverDataInfo[0], out serverID) || !int.TryParse(serverDataInfo[30], out registMaxNum) || !int.TryParse(serverDataInfo[28], out onlineMaxNum))
                {
                    continue;
                }
                if (serverID <= 0 || registMaxNum <= 0 || onlineMaxNum <= 0)
                {
                    continue;
                }

                //需要创建不同的引用对象

                ServerInfo newData1 = new ServerInfo();
                newData1.serverID = serverID; //服务器id
                newData1.serverName = serverDataInfo[2];  //服务器名称
                newData1.registMaxNum = registMaxNum; //最大注册人数
                newData1.onlineMaxNum = onlineMaxNum;//最大在线人数
                //添加到 服务器 
                list_ServerData.Add(newData1);

                //添加到 登陆服数据 列表中
                list_LoginData.Add(newData1.Copy()); //需创建新对象

                //GameLog.LogFormat("加载完成ServerConfigList配置文件，其中 serverID:{0}, serverName:{1}, registMaxNum:{2}, onlineMaxNum:{3}",
                //    newData.serverID, newData.serverName, newData.registMaxNum, newData.onlineMaxNum);
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
                if (!int.TryParse(serverDataInfo[0], out configID))
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
            GameLog.LogFormat("加载完成config配置文件，其中 configID:{0}, addRegNum:{1}, addLoginNum:{2}, delLoginNum:{3}, syncTime:{4}, queueMaxLimit:{5}, loginConfirmDeLay:{6}",
                        emulatorConfig.configID, emulatorConfig.addRegNum, emulatorConfig.addLoginNum, emulatorConfig.delLoginNum,
                        emulatorConfig.syncTime, emulatorConfig.queueMaxLimit, emulatorConfig.loginConfirmDeLay);
        }
        #endregion

        #region 模拟器相关接口

        public EmulatorConfig GetEmulatorConfig()
        {
            return emulatorConfig;
        }

        /// <summary>
        /// 是否打开了 模拟器开关
        /// </summary>
        /// <returns></returns>
        public bool IsOpenEmulator()
        {
            return isOpenEmulator;
        }

        /// <summary>
        /// 设置 模拟器开关
        /// </summary>
        /// <param name="newSet"></param>
        public void SetOpenEmulator(bool newSet)
        {
            isOpenEmulator = newSet;
        }

        /// <summary>
        /// 每隔一定时间 需要同步 登录服和服务器的数据
        /// </summary>
        private void SyncData()
        {
            //将最新的 list_ServerData的值 赋值给 list_LoginData
            for (int i = 0; i < list_LoginData.Count; i++)
            {
                list_LoginData[i] = list_ServerData[i].Copy();
            }
        }

        /// <summary>
        /// 每秒执行
        /// </summary>
        public void OnSecond()
        {
            if (!isOpenEmulator) {
                return;
            }
            //GameLog.Log("---------------------------------- 模拟器 OnSecond 每秒日志 --------------------------------------");

            //(1) 秒计数++
            SecondCount++;
            if (SecondCount % emulatorConfig.syncTime == 0)
            {
                //每隔 SyncTime 秒，同步服务器和登陆服数据
                SyncData();
            }

            //(2) 单服务器 逻辑处理
            for (int i = 0; i < list_ServerData.Count; i++)
            {
                //每秒登录 addLoginNum 人
                list_ServerData[i].onlineNum += emulatorConfig.addLoginNum;
                //修正登录数据  在线人数不能大于 总注册人数
                if (list_ServerData[i].onlineNum >= list_ServerData[i].registedNum)
                {
                    list_ServerData[i].onlineNum = list_ServerData[i].registedNum;
                }

                //与最大在线人数比对，多的添加到排队
                int tempAddQueue = list_ServerData[i].onlineNum - list_ServerData[i].onlineMaxNum;
                if (tempAddQueue > 0)
                {
                    //添加到排队中
                    list_ServerData[i].queueNum += tempAddQueue;

                    if (list_ServerData[i].queueNum >= list_ServerData[i].queueMaxNum)
                    {
                        //排队人数已满
                        list_ServerData[i].queueNum = list_ServerData[i].queueMaxNum;
                    }
                }

                //在线人数已满
                if (list_ServerData[i].onlineNum >= list_ServerData[i].onlineMaxNum)
                {
                    list_ServerData[i].onlineNum = list_ServerData[i].onlineMaxNum;
                }

                //每秒登出 delLoginNum 人
                list_ServerData[i].onlineNum -= emulatorConfig.delLoginNum;
                if (list_ServerData[i].onlineNum <= 0)
                {
                    list_ServerData[i].onlineNum = 0;
                }
            }

            //(3) 新增注册人数 addRegNum人 -------->算出每个服务器的regRate，找出最大值分配服务器，再更新服务器、登录服已注册人数
            for (int i = 0; i < emulatorConfig.addRegNum; i++)
            {
                //a. 获取服务器的最大 regRate
                RegRateInfo max_regRateInfo = GetMaxRateInfo();
                if (max_regRateInfo == null)
                {
                    //无可用服务器，直接退出循环
                    break;
                }

                //找到的 max_regRate 为零，则查找分配失败，无可用服务器
                if (max_regRateInfo.regRate <= 0)
                {
                    //无可用服务器，直接退出循环
                    break;
                }

                //b. 分配完毕逻辑 更新服务器、登录服已注册人数
                SetRegistAddOne(max_regRateInfo.serverID);

                GameLog.Log("分配成功一个人到服务器serverID: " + max_regRateInfo.serverID + ",  regRate: " + max_regRateInfo.regRate + ", 其最新已注册人数为:" + GetServerDataByServerID(max_regRateInfo.serverID).registedNum);
            }

            //(4)输出当前所有服的各项数据:
            for (int i = 0; i < list_LoginData.Count; i++)
            {
                //if (i == 0)
                //{
                //    GameLog.LogFormat("serverID:{0},  onlineNum:{1}, queueNum:{2},registedNum:{3}",
                //        list_LoginData[i].serverID, list_LoginData[i].onlineNum, list_LoginData[i].queueNum, list_LoginData[i].registedNum);
                //}
                GameLog.LogFormat("serverID:{0},  onlineNum:{1}, queueNum:{2},registedNum:{3}",
                    list_LoginData[i].serverID, list_LoginData[i].onlineNum, list_LoginData[i].queueNum, list_LoginData[i].registedNum);
            }
        }

        /// <summary>
        /// 根据服务器id, 计算对应的注册权重 regRate 
        /// 参考: readme\跨服中角色分配逻辑.txt
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>

        private float GetRegRateByServerID(int serverID)
        {
            //服务器数据
            ServerInfo serverInfo_server = GetServerDataByServerID(serverID);
            //登录服数据
            ServerInfo serverInfo_login = GetLoginDataByServerID(serverID);

            int curRegistedNum = Math.Max(serverInfo_server.registedNum, serverInfo_login.registedNum);

            //用登录服的各项数据计算 注册权重 regRate 
            int regAvail = serverInfo_login.registMaxNum - curRegistedNum - serverInfo_login.queueNum;
            if (serverInfo_login.onlineMaxNum + serverInfo_login.queueMaxNum == 0)
            {
                return 0f;
            }
            float stateRate = (serverInfo_login.onlineNum + serverInfo_login.queueNum) / (serverInfo_login.onlineMaxNum + serverInfo_login.queueMaxNum);
            float regRate = (1 - stateRate) * regAvail;
            return regRate;
        }

        /// <summary>
        /// 注册分配完毕一个玩家处理
        /// </summary>
        /// <param name="serverID"></param>
        private void SetRegistAddOne(int serverID)
        {
            //服务器数据
            ServerInfo serverInfo_server = GetServerDataByServerID(serverID);
            //登录服数据
            ServerInfo serverInfo_login = GetLoginDataByServerID(serverID);

            //已注册人数 ++ 
            serverInfo_server.registedNum++;
            serverInfo_login.registedNum++;
        }

        /// <summary>
        /// 获取当前 最高权重的服务器
        /// </summary>
        /// <returns></returns>
        public RegRateInfo GetMaxRateInfo()
        {
            float max_regRate = float.MinValue;
            int max_serverID = -1;
            for (int i = 0; i < list_LoginData.Count; i++)
            {
                if (!list_LoginData[i].isOpenReg)
                {
                    continue;
                }
                float regRate = GetRegRateByServerID(list_LoginData[i].serverID);
                //保存较大的权重即可
                if (max_regRate < regRate)
                {
                    max_regRate = regRate;
                    max_serverID = list_LoginData[i].serverID;
                }
            }

            RegRateInfo regRateInfo = new RegRateInfo();
            regRateInfo.regRate = max_regRate;
            regRateInfo.serverID = max_serverID;
            return regRateInfo;
        }

        /// <summary>
        /// 获取当前 最低权重的服务器
        /// </summary>
        /// <returns></returns>
        public RegRateInfo GetMinRateInfo()
        {
            float min_regRate = float.MaxValue;
            int min_serverID = -1;
            for (int i = 0; i < list_LoginData.Count; i++)
            {
                if (!list_LoginData[i].isOpenReg)
                {
                    continue;
                }
                float regRate = GetRegRateByServerID(list_LoginData[i].serverID);
                //保存较小的权重即可
                if (min_regRate > regRate)
                {
                    min_regRate = regRate;
                    min_serverID = list_LoginData[i].serverID;
                }
            }

            RegRateInfo regRateInfo = new RegRateInfo();
            regRateInfo.regRate = min_regRate;
            regRateInfo.serverID = min_serverID;
            return regRateInfo;
        }

        #endregion
    }
}
