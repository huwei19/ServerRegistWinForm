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
        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            DisplayLog("listView1_ColumnWidthChanging_1");
            /// todo 后续再改
            //Rectangle SizeR = default(Rectangle);
            //int width = e.NewWidth;
            //foreach (Control item in lv.Controls)
            //{
            //    //根据名字找到所有的progressbar
            //    if (item.Name.IndexOf("progressbar") >= 0)
            //    {
            //        ProgressBar bar = (ProgressBar)item;
            //        //Rectangle size=bar.Bounds;
            //        SizeR = bar.Bounds;
            //        //lv.Columns[2]是放置progressbar的地方
            //        SizeR.Width = lv.Columns[2].Width;
            //        bar.SetBounds(lv.Items[0].SubItems[2].Bounds.X, SizeR.Y, SizeR.Width, SizeR.Height);
            //        //bar.Width = width;
            //    }
            //}
        }

        //初始化列表
        private void IniListView()
        {
            //添加列头，设置宽度，显示位置
            listView1.Columns.Add("服务器id", 120, HorizontalAlignment.Center);
            listView1.Columns.Add("进度条", 120, HorizontalAlignment.Center);
            listView1.Columns.Add("注册人数占比", 200, HorizontalAlignment.Center);
            listView1.Columns.Add("关闭注册按钮", 200, HorizontalAlignment.Center);

            //设置属性
            listView1.GridLines = true;//显示网格线
            listView1.HideSelection = false;//失去焦点时显示当前选择的项
            listView1.HoverSelection = false;//当鼠标指针停留数秒时自动选择项
            listView1.FullRowSelect = false; //选中一行
            listView1.View = View.Details;

            //加载 表格数据
            AddDataAndPic();
        }

        //ListView中添加数据、图片
        private void AddDataAndPic()
        {

            ListViewItem item = new ListViewItem(new string[] { "1", "2", "3", "" });
            listView1.Items.Add(item);


            //todo 循环添加进度条和关闭按钮
            //添加进度条
            float progress = 20;
            Rectangle SizeR = default(Rectangle);
            ProgressBar ProgBar = new ProgressBar();
            SizeR = item.SubItems[1].Bounds;
            SizeR.Width = listView1.Columns[1].Width;
            ProgBar.Parent = listView1;
            ProgBar.SetBounds(SizeR.X, SizeR.Y, SizeR.Width, SizeR.Height);
            ProgBar.Value = (int)progress;
            ProgBar.Visible = true;
            //取一个唯一的名字，以后好找
            ProgBar.Name = "1 progressbar";

            //添加 关闭注册 按钮
            Button button = new Button();
            SizeR = item.SubItems[3].Bounds;
            button.Parent = listView1;
            button.SetBounds(SizeR.X, SizeR.Y, 65, 23);
            button.Visible = true;
            //取一个唯一的名字，以后好找
            button.Name = "1 button";
            button.Text = "开注册";
            //将关闭按钮都 绑定同一个点击事件 利用点击的物体名判定 点击到了哪个
            button.Click += new EventHandler(buttonClose_Click);

        }

        //修改ListView数据
        private void UpdateDate()
        {
            listView1.Items[0].SubItems[2].Text = "哈哈1";
            listView1.Items[1].SubItems[2].Text = "哈哈2";
            listView1.Items[2].SubItems[2].Text = "哈哈3";
        }

        //将关闭注册 点击事件
        private void buttonClose_Click(object sender, EventArgs e)
        {
            //todo 通过名称 判定点击到了服务器
            Button button = (Button)sender;
            DisplayLog("点击了按钮: " + button.Name.ToString());
        }
    }
}
