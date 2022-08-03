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
        /// <summary>
        /// 更新 listview中 控件位置
        /// </summary>
        private void UpdateListViewLocal()
        {
            Rectangle SizeR = default(Rectangle);
            //循环列表的每行数据
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                //先拿对应行的 服务器id
                int serverID = -1;
                if (!int.TryParse(listView1.Items[i].SubItems[0].Text, out serverID))
                {
                    continue;
                }

                ServerInfo serverInfo = EmulatorManager.GetInstance().GetServerDataByServerID(serverID);
                if (serverInfo == null)
                {
                    continue;
                }

                //进度条位置 更新
                Control[] progressbars = listView1.Controls.Find("progressbar" + i, true);
                if (progressbars == null || progressbars[0] == null)
                {
                    continue;
                }
                //修改 ProgressBar 属性
                ProgressBar bar = (ProgressBar)progressbars[0];
                SizeR = listView1.Items[i].SubItems[1].Bounds;
                SizeR.Width = listView1.Columns[1].Width;
                bar.SetBounds(SizeR.X, SizeR.Y, SizeR.Width, SizeR.Height);

                //button位置 更新
                Control[] registBtns = listView1.Controls.Find("registBtn" + i, true);
                if (registBtns == null || registBtns[0] == null)
                {
                    continue;
                }
                Button button = (Button)registBtns[0];
                SizeR = listView1.Items[i].SubItems[3].Bounds;
                button.SetBounds(SizeR.X, SizeR.Y, 100, SizeR.Height);
            }
        }

        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            DisplayLog("listView1_ColumnWidthChanging_1");
        }

        //初始化列表
        private void InitListView()
        {
            //添加列头，设置宽度，显示位置
            listView1.Columns.Add("服务器id", 120, HorizontalAlignment.Center);
            listView1.Columns.Add("进度条", 200, HorizontalAlignment.Center);
            listView1.Columns.Add("已注册人数占比", 100, HorizontalAlignment.Center);
            listView1.Columns.Add("注册按钮", 100, HorizontalAlignment.Center);

            //设置属性
            listView1.AllowColumnReorder = false;
            listView1.GridLines = true;//显示网格线
            listView1.FullRowSelect = false; //选中一行
            listView1.View = View.Details;

            //用SmallImageList来设置行高
            ImageList ImgList = new ImageList();
            //高度设为25
            ImgList.ImageSize = new Size(1, 20);
            //在Details显示模式下，小图标才会起作用
            listView1.SmallImageList = ImgList;

            //加载 表格数据
            AddDataAndPic();
        }

        //ListView中添加数据、图片
        private void AddDataAndPic()
        {
            //循环各自的服务器数据
            List<ServerInfo> serverInfos = EmulatorManager.GetInstance().GetServerDatas();
            DisplayLog("总共服务器行数: " + serverInfos.Count);
            for (int i = 0; i < serverInfos.Count; i++)
            {
                //初始给默认的 四列
                ListViewItem item = new ListViewItem(new string[] { "", "", "", "" });
                listView1.Items.Add(item);

                //服务器 id
                item.SubItems[0].Text = serverInfos[i].serverID.ToString();

                //进度条
                float progress = 0;
                Rectangle SizeR = default(Rectangle);
                ProgressBar ProgBar = new ProgressBar();
                SizeR = item.SubItems[1].Bounds;
                SizeR.Width = listView1.Columns[1].Width;
                ProgBar.Parent = listView1;
                ProgBar.SetBounds(SizeR.X, SizeR.Y, SizeR.Width, SizeR.Height);
                ProgBar.Value = (int)progress;
                ProgBar.Maximum = serverInfos[i].registMaxNum;
                ProgBar.Visible = true;
                //取一个唯一的名字，以后好找 ( progressbar + 行索引)
                ProgBar.Name = "progressbar" + i;


                //已注册人数占比  已注册人数/最大注册人数
                item.SubItems[2].Text = serverInfos[i].registedNum + "/" + serverInfos[i].registMaxNum;

                //添加 关闭注册 按钮
                Button button = new Button();
                SizeR = item.SubItems[3].Bounds;
                button.Parent = listView1;
                button.SetBounds(SizeR.X, SizeR.Y, 100, SizeR.Height);
                button.Visible = true;
                //取一个唯一的名字，以后好找 ( registBtn + 行索引)
                button.Name = "registBtn" + i;
                if (serverInfos[i].isOpenReg)
                {
                    button.Text = "开注册" + i;
                    button.BackColor = Color.MediumSpringGreen;
                }
                else
                {
                    button.Text = "关注册" + i;
                    button.BackColor = Color.OrangeRed;
                }
                //DisplayLog("创建按钮 Name:" + button.Name + ", text:" + button.Text);
                //将关闭按钮都 绑定同一个点击事件 利用点击的物体名判定 点击到了哪个
                button.Click += new EventHandler(buttonClose_Click);
            }
        }

        //将关闭注册 点击事件
        private void buttonClose_Click(object sender, EventArgs e)
        {
            //通过名称 判定点击到了服务器
            Button button = (Button)sender;
            DisplayLog("点击了按钮: " + button.Name.ToString());
            if (!button.Name.Contains("registBtn"))
            {
                return;
            }
            string str = button.Name.Remove(0, "registBtn".Length);
            int lineIndex = -1;
            if (!int.TryParse(str, out lineIndex) || lineIndex < 0)
            {
                return;
            }

            //先拿对应行的 服务器id
            int serverID = -1;
            if (!int.TryParse(listView1.Items[lineIndex].SubItems[0].Text, out serverID))
            {
                return;
            }

            ServerInfo serverInfo = EmulatorManager.GetInstance().GetServerDataByServerID(serverID);
            if (serverInfo == null)
            {
                return;
            }
            
            if (serverInfo.isOpenReg)
            {
                serverInfo.isOpenReg = false;
                button.Text = "关注册" + lineIndex;
                button.BackColor = Color.OrangeRed;
            }
            else
            {
                serverInfo.isOpenReg = true;
                button.Text = "开注册" + lineIndex;
                button.BackColor = Color.MediumSpringGreen;
            }
        }

        //修改展示View数据
        private void UpdateListView()
        {
            //循环列表的每行数据
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                //先拿对应行的 服务器id
                int serverID = -1;
                if (!int.TryParse(listView1.Items[i].SubItems[0].Text, out serverID))
                {
                    continue;
                }

                ServerInfo serverInfo = EmulatorManager.GetInstance().GetServerDataByServerID(serverID);
                if (serverInfo == null)
                {
                    continue;
                }

                //已注册人数占比 更新
                listView1.Items[i].SubItems[2].Text = serverInfo.registedNum + "/" + serverInfo.registMaxNum;

                //进度条更新
                Control[] items = listView1.Controls.Find("progressbar" + i, true);
                if (items == null || items[0] == null)
                {
                    continue;
                }
                //修改 ProgressBar 属性
                ProgressBar bar = (ProgressBar)items[0];
                bar.Value = serverInfo.registedNum;
            }
        }
    }
}
