using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerRegistWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DisplayLog("表格加载");

            //初始化列表
            InitListView();

            //每秒新增注册数
            label2.Text = EmulatorManager.GetInstance().GetEmulatorConfig().addRegNum.ToString();

            //每秒单服登录数
            label4.Text = EmulatorManager.GetInstance().GetEmulatorConfig().addLoginNum.ToString();

            //每秒单服登出数
            label6.Text = EmulatorManager.GetInstance().GetEmulatorConfig().delLoginNum.ToString();

            //服务器数据同步频率
            label8.Text = EmulatorManager.GetInstance().GetEmulatorConfig().syncTime.ToString();

            //单服排队上限
            label10.Text = EmulatorManager.GetInstance().GetEmulatorConfig().queueMaxLimit.ToString();

            //登录确认延时
            label12.Text = EmulatorManager.GetInstance().GetEmulatorConfig().loginConfirmDeLay.ToString();

            //最高权重的服务器
            //最低权重的服务器
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //模拟器开始按钮点击
        private void button1_Click(object sender, EventArgs e)
        {
            DisplayLog("模拟器 开始按钮点击");
            EmulatorManager.GetInstance().SetOpenEmulator(true);
        }

        //模拟器结束按钮点击
        private void button2_Click(object sender, EventArgs e)
        {
            DisplayLog("模拟器 结束按钮点击");
            EmulatorManager.GetInstance().SetOpenEmulator(false);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
        private void DisplayLog(string str)
        {
            textBox1.AppendText(DateTime.Now.ToString("HH:mm:ss   ") + str + "\r\n");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_2(object sender, EventArgs e)
        {

        }


        //秒更新 Time空间
        private void timer1_Tick(object sender, EventArgs e)
        {
            //定时器每秒执行
            //DisplayLog("定时器每秒执行");
            EmulatorManager.GetInstance().OnSecond();

            //最高权重的服务器 显示更新
            RegRateInfo maxRegRateInfo = EmulatorManager.GetInstance().GetMaxRateInfo();
            label14.Text = maxRegRateInfo.serverID + "(" + maxRegRateInfo.regRate + ")";

            //最低权重的服务器 显示更新
            RegRateInfo minRegRateInfo = EmulatorManager.GetInstance().GetMinRateInfo();
            label16.Text = minRegRateInfo.serverID + "(" + minRegRateInfo.regRate + ")";

            //更新 listView 显示
            UpdateListView();
        }

        //0.1秒更新 time时间
        private void timer2_Tick(object sender, EventArgs e)
        {
            //更新 listView 显示位置
            UpdateListViewLocal();
        }


    }
}
