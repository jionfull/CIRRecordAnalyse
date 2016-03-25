using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using CIRRecordAnalyse.Core;

namespace CIRRecordAnalyse
{
    public partial class MultiSearch : Form                //DevExpress.XtraEditors.XtraForm
    {

        SearchCondition searchCond = new SearchCondition();
        List<SearchCondition> searchCondList = new List<SearchCondition>();

        //用于按端口...查询的int数组
        List<int> int_SrcList = new List<int>();               //源端口->条件
        List<int> int_DstList = new List<int>();
        List<int> int_typeList = new List<int>();
        List<int> int_commandList = new List<int>();

        int searchPage = 0;         //当前使用的是<具体类型>还是<端口...搜索>
        //
        int[][] SCond2 = new int[][] { 
        new int[]{  1,3,8},  new int[]{-1,3,9},   new int[]{-1,3,10},  new int[]{-1,3,0x11},new int[]{-1,3,0x13},
        new int[]{-1,3,0x15},new int[]{-1,3,0x16},new int[]{-1,3,0x20},new int[]{-1,3,0x22},new int[]{-1,3,0x2e},
        new int[]{-1,3,0x42},new int[]{-1,3,0x4a},new int[]{-1,3,0x4b},new int[]{-1,3,0x4c},new int[]{-1,3,0x52},
        new int[]{-1,3,0x54},new int[]{-1,3,0x55},new int[]{-1,3,0x58},new int[]{-1,3,0x59},new int[]{-1,3,0x90},
        new int[]{-1,3,0x5b},new int[]{-1,3,0x5d},new int[]{-1,3,0x5f},new int[]{-1,6,0x20},new int[]{-1,6,0x21},
        new int[]{-1,6,0x22},new int[]{-1,6,0x23},new int[]{-1,6,0x51},new int[]{-1,6,0x53},new int[]{-1,4,0x21},
        new int[]{-1,4,0x22},new int[]{-1,4,0x23},new int[]{-1,4,0x24},new int[]{-1,4,0x25},new int[]{-1,4,0x26},
        new int[]{6,2,-1},   new int[]{-1,11,-1}, new int[]{0x13,-1,-1},
        };

        readonly static int[] checkToPort = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 0x11, 0x12, 0x13, 0x14, 0x21, 0x23, 0x24, 0x25, 0x26, 0x3f };     //checkbox 转 源端口号
        readonly static int[] checkToType = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e };   

        public MultiSearch()
        {
            InitializeComponent();
        }
        //-----------------------------------------------
        public int SearchPage
        {
            get { return searchPage; }
            set { searchPage = value; }
        }
        public List<int> srcPortInt { get { return int_SrcList; } }
        public List<int> dstPortInt { get { return int_DstList; } }
        public List<int> TypeInt { get { return int_typeList; } }
        public List<int> CommandInt { get { return int_commandList; } }
        //-----------------------------------------------

        private void CancelSearch_Click(object sender, EventArgs e)
        {
            searchPage = 0;
            //Hide();
            Close();
        }

        private void OK_Button(object sender, EventArgs e)
        {
            GoToSearch();
        }
        private void GoToSearch()
        {
            if (xtraTabControl1.SelectedTabPageIndex == 0)      //选择标签页:按类型多选搜索
            {
                searchCondList.Clear();
                for (int checkcount = 0; checkcount < checkedListBoxControl1.ItemCount; checkcount++)
                {
                    if (checkedListBoxControl1.GetItemChecked(checkcount))
                    {
                        searchCond = new SearchCondition();
                        searchCond.Port = SCond2[checkcount][0];
                        searchCond.Style = SCond2[checkcount][1];
                        searchCond.Command = SCond2[checkcount][2];
                        searchCondList.Add(searchCond);
                    }
                }
                searchPage = 1;
            }
            //------------------------------------------------------
            else if (xtraTabControl1.SelectedTabPageIndex == 1)             //按端口
            {
                int_SrcList = new List<int>();               //清空?
                int_DstList = new List<int>();
                int_typeList = new List<int>();
                int_commandList = new List<int>();
                //
                for (int i = 0; i < checkedListBoxControl2.ItemCount; i++)
                {
                    if (checkedListBoxControl2.GetItemChecked(i))        //获取源端口选项
                        int_SrcList.Add(checkToPort[i]);
                    if (checkedListBoxControl3.GetItemChecked(i))
                        int_DstList.Add(checkToPort[i]);
                    if (i < checkedListBoxControl4.ItemCount)
                    {
                        if (checkedListBoxControl4.GetItemChecked(i))
                            int_typeList.Add(checkToType[i]);
                    }
                }
                //
                string[] str_addCommand = textEdit_commmand.Text.Split(',');                       //Add New Commands
                for (int ii = 0; ii < str_addCommand.Length; ii++)
                {
                    try { int int_addCom = int.Parse(str_addCommand[ii], System.Globalization.NumberStyles.AllowHexSpecifier); int_commandList.Add(int_addCom); }
                    catch { }
                }
                //
                if (checkEdit1.Checked)           //(可选)新增端口,类型
                {
                    string[] str_addSrc = textEdit_srcPort.Text.Split(',');
                    string[] str_addDst = textEdit_dstPort.Text.Split(',');
                    string[] str_addType = textEdit_type.Text.Split(',');
                    for (int ii = 0; ii < str_addSrc.Length; ii++)
                    {
                        try { int int_addSrc = int.Parse(str_addSrc[ii], System.Globalization.NumberStyles.AllowHexSpecifier); int_SrcList.Add(int_addSrc); }
                        catch { }
                    }
                    for (int ii = 0; ii < str_addDst.Length; ii++)
                    {
                        try { int int_addDst = int.Parse(str_addDst[ii], System.Globalization.NumberStyles.AllowHexSpecifier); int_DstList.Add(int_addDst); }
                        catch { }
                    }
                    for (int ii = 0; ii < str_addType.Length; ii++)
                    {
                        try { int int_addType = int.Parse(str_addType[ii], System.Globalization.NumberStyles.AllowHexSpecifier); int_typeList.Add(int_addType); }
                        catch { }
                    }
                }
                searchPage = 2;
            }
            else 
                searchPage = 0;
            //Hide();
            Close();
        }

        public void GetSearchCondList(ref List<SearchCondition> scl)            //传递SearchCondition数组数据
        {
            scl = searchCondList;
        }
        //----------------------用户界面Optimize----------------------
        private void MultiSearch_Load(object sender, EventArgs e)
        {
            xtraTabControl1.SelectedTabPage = xtraTabPage2;
        }

        private void checkedListBox_ForAll_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)       //显示已选条件_原函数
        {
            ShowConditionInfo();
        }

        private void ShowConditionInfo()       //显示已选条件_调用ShowConditionInfo()
        {
            richTextBox1.Text = "   源端口:";
            for (int a = 0; a < checkedListBoxControl2.Items.Count; a++)
                richTextBox1.Text += ((checkedListBoxControl2.Items[a].CheckState == CheckState.Checked) ? (checkedListBoxControl2.Items[a].Description + ",") : "");
            richTextBox1.Text = richTextBox1.Text.Trim(',');
            richTextBox1.Text += "\n   目的端口:";
            for (int a = 0; a < checkedListBoxControl3.Items.Count; a++)
                richTextBox1.Text += ((checkedListBoxControl3.Items[a].CheckState == CheckState.Checked) ? (checkedListBoxControl3.Items[a].Description + ",") : "");
            richTextBox1.Text = richTextBox1.Text.Trim(',');
            richTextBox1.Text += "\n   业务类型:";
            for (int a = 0; a < checkedListBoxControl4.Items.Count; a++)
                richTextBox1.Text += ((checkedListBoxControl3.Items[a].CheckState == CheckState.Checked) ? (checkedListBoxControl3.Items[a].Description + ",") : "");
            richTextBox1.Text = richTextBox1.Text.Trim(',');
        }

        private void MultiSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            //searchPage = 0;
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            textEdit_srcPort.Enabled = textEdit_dstPort.Enabled = textEdit_type.Enabled = checkEdit1.Checked;
            if (checkEdit1.Checked) textEdit_srcPort.Text = textEdit_dstPort.Text = textEdit_type.Text = "";
            else
            {
                textEdit_srcPort.Text = "新增自定义端口或业务类型";
                textEdit_dstPort.Text = "使用16进制,以逗号隔开";
                textEdit_type.Text = "如:01,2,a1,13";
            }
        }

        private void textEdit_commmand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GoToSearch();
            }
        }
        //
    }
}