using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRegistWinForm
{
    class EmulatorEntity
    {
    }

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

        /// <summary>
        /// 拷贝
        /// </summary>
        /// <returns></returns>
        public ServerInfo Copy() {
            ServerInfo copy = (ServerInfo)this.MemberwiseClone();
            return copy;
        }
    }

    public class RegRateInfo
    {
        //服务器id
        public int serverID = 0;

        public float regRate = 0;
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
}
