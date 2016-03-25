using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CIRRecordAnalyse.Core;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace CIRRecordAnalyse
{
    public partial class FormMain : Form
    {     

        SearchCondition searchCond = new SearchCondition();
        SearchCondition searchStatusCond = new SearchCondition();
        SearchCondition searchVoiceCond = new SearchCondition();

        private RecordSerial sidebarRS;             //侧边栏信息
        MultiSearch mts;
        int lastSearchAction = -1;
        List<RecordManager> rl = new List<RecordManager>();

        delegate void searchWithTime(object sender, DevExpress.XtraBars.ItemClickEventArgs e);

        readonly static string[] CommonType = new string[] { "综合信息", "网络状态", "调度命令", "列尾数据", "GPS数据", "LBJ数据" };
        int[][] SCond1 = new int[][] {  new int[] { -1, 3, 0x55 }, 
                                        new int[] { -1, 3, 0x54 }, 
                                        new int[] { -1, 6, -1 }, 
                                        new int[] { -1, 4, -1 }, 
                                        new int[] { 6, 2, -1 }, 
                                        new int[] { 0x13, 11, -1 }, };
        readonly static string[] DetailType = new string[] { 
        "MMI选择工作模式", "MMI摘/挂机", "MMI(PTT)操作", "MMI设置车次号", "MMI GSM-R呼叫操作", "MMI接通保持电话", "MMI(呼叫转移选择)", "MMI设置机车号", "MMI按键呼叫(450MHz)", "MMI设置库检状态", "主机分配端口号", "主机报告450MHz场强", "主机确认450MHz呼叫", "主机指示450MHz来呼", "主机确认Hook,PTT", "主机报告网络状态", 
        "主机报告综合信息", "主机报告车次号状态", "主机报告机车号状态", "主机报告AC确认", "主机确认G网呼叫", "主机通知通话列表", "主机通知库检结果", "主机上传调度命令", "上传10条最新索引", "上传前10条索引", "上传后10条索引", "MMI发送确认签收信息", "MMI发送调车请求", "风压信息", "排风信息", "风压报警信息", 
        "电压报警信息", "建立对应关系", "拆除对应关系", "GPS定位信息", "LBJ的数据类型", "LBJ的端口数据","MMI之间按键信息","MMI之间内部调试信息","MMI之间“确认/签收”按键信息","MMI之间设置信息"
        };
        int[][] SCond2 = new int[][] { 
        new int[]{  1,3,8},  new int[]{-1,3,9},   new int[]{-1,3,10},  new int[]{-1,3,0x11},new int[]{-1,3,0x13},
        new int[]{-1,3,0x15},new int[]{-1,3,0x16},new int[]{-1,3,0x20},new int[]{-1,3,0x22},new int[]{-1,3,0x2e},
        new int[]{-1,3,0x42},new int[]{-1,3,0x4a},new int[]{-1,3,0x4b},new int[]{-1,3,0x4c},new int[]{-1,3,0x52},
        new int[]{-1,3,0x54},new int[]{-1,3,0x55},new int[]{-1,3,0x58},new int[]{-1,3,0x59},new int[]{-1,3,0x90},
        new int[]{-1,3,0x5b},new int[]{-1,3,0x5d},new int[]{-1,3,0x5f},new int[]{-1,6,0x20},new int[]{-1,6,0x21},
        new int[]{-1,6,0x22},new int[]{-1,6,0x23},new int[]{-1,6,0x51},new int[]{-1,6,0x53},new int[]{-1,4,0x21},
        new int[]{-1,4,0x22},new int[]{-1,4,0x23},new int[]{-1,4,0x24},new int[]{-1,4,0x25},new int[]{-1,4,0x26},
        new int[]{6,2,-1},   new int[]{-1,11,-1}, new int[]{0x13,-1,-1},new int[]{-1,3,0x19,0xaa},new int[]{-1,3,0x19,0xab},
        new int[]{-1,3,0x19,0xac},new int[]{-1,3,0x19,0x0F}
        };
        FormRecordList formList = new FormRecordList();
        public FormMain()
        {           
            InitializeComponent();
        }

        //public static Process GetRunningInstance()
        //{
        //    Boolean mutexWasCreated;//声明一个Boolean值,用于下面的Out
        //    Process currentProcess = Process.GetCurrentProcess(); //获取当前进程   
        //    string zz = currentProcess.ProcessName;
        //    Mutex myMutex = new Mutex(true, "zz", out mutexWasCreated);
        //    if (!mutexWasCreated)//对返回值进行判断             
        //    { 
        //        MessageBox.Show("程序已处于运行中，请不要重复运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop); 
        //        Application.Exit();//退出系统 
        //        return null;
        //        //return;//因为Application.Exit();会处理完消息系统才退出程序,所以直接retrun出去让他结束             
        //    }            
        //    return currentProcess;//返回已运行的进程实例 
        //}  


        public RecordSerial SidebarRS
        {
            set { sidebarRS = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            repositoryItemComboBox1.Items.AddRange(CommonType);
            repositoryItemComboBox2.Items.AddRange(DetailType);
            formList.MdiParent = this;
            formList.Show();
        }

        private void xtraTabbedMdiManager1_PageAdded(object sender, DevExpress.XtraTabbedMdi.MdiTabPageEventArgs e)
        {
            if (e.Page.MdiChild is FormRecordList)
            {
                e.Page.ShowCloseButton = DevExpress.Utils.DefaultBoolean.False;
                e.Page.ImageIndex = 0;
            }
            else if (e.Page.MdiChild is FormSerialRecord)
            {
                e.Page.ImageIndex = 1;
            }
            else if (e.Page.MdiChild is FormStatusRecord)
            {
                e.Page.ImageIndex = 2;
            }
            else if (e.Page.MdiChild is FormVoiceRecord)
            {
                e.Page.ImageIndex = 3;
            }
        }

        private void xtraTabbedMdiManager1_SelectedPageChanged(object sender, EventArgs e)
        {

            Form formSel = xtraTabbedMdiManager1.SelectedPage.MdiChild;
            if (formSel is FormRecordList)
            {
                ribbonControl1.SelectedPage = ribbonPage1;
            }
            else if (formSel is FormSerialRecord)
            {
                ribbonPage2.Visible = true;
                ribbonControl1.SelectedPage = ribbonPage2;
                FormSerialRecord fsr = formSel as FormSerialRecord;
                barEditItem4.EditValue = fsr.SearchBeginTime;
                barEditItem5.EditValue = fsr.SearchEndTime;
            }
            else if (formSel is FormStatusRecord)
            {
                ribbonPage3.Visible = true;
                ribbonControl1.SelectedPage = ribbonPage3;
                FormStatusRecord fsr = formSel as FormStatusRecord;
                barEditItem6.EditValue = fsr.SearchBeginTime;
                barEditItem7.EditValue = fsr.SearchEndTime;
            }
            else if (formSel is FormVoiceRecord)
            {
                ribbonPage4.Visible = true;
                ribbonControl1.SelectedPage = ribbonPage4;
                FormVoiceRecord fvr = formSel as FormVoiceRecord;
                barEditItem8.EditValue = fvr.SearchBeginTime;
                barEditItem9.EditValue = fvr.SearchEndTime;
            }
        }

        private void xtraTabbedMdiManager1_PageRemoved(object sender, DevExpress.XtraTabbedMdi.MdiTabPageEventArgs e)
        {
            bool hasSerialRecord = false;
            bool hasStatusRecord = false;
            bool hasVoiceRecord = false;
            for (int i = 0; i < xtraTabbedMdiManager1.Pages.Count; i++)
            {
                if (xtraTabbedMdiManager1.Pages[i].MdiChild is FormSerialRecord)
                {
                    hasSerialRecord = true;
                }
                else if (xtraTabbedMdiManager1.Pages[i].MdiChild is FormStatusRecord)
                {
                    hasStatusRecord = true;
                }
                else if (xtraTabbedMdiManager1.Pages[i].MdiChild is FormVoiceRecord)
                {
                    hasVoiceRecord = true;
                }
            }
            if (hasSerialRecord == false)
            {
                ribbonPage2.Visible = false;
            }
            if (hasStatusRecord == false)
            {
                ribbonPage3.Visible = false;
            }
            if (hasVoiceRecord == false)
            {
                ribbonPage4.Visible = false;
            }
        }

        private void buttonAutoSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)//自动查找
        {
            DriveInfo[] drvInfos = DriveInfo.GetDrives();       // Get a DriveInfo object for each drive on the system
            for (int i = 0; i < drvInfos.Length; i++)
            {
                if (drvInfos[i].IsReady && (drvInfos[i].DriveFormat == "FAT" || drvInfos[i].DriveFormat == "FAT32") && drvInfos[i].DriveType == DriveType.Removable)
                {
                    if (File.Exists(Path.Combine(drvInfos[i].Name, "MENU.bin")) == false) continue;
                    if (File.Exists(Path.Combine(drvInfos[i].Name, "DATA.bin")) == false) continue;
                    if (File.Exists(Path.Combine(drvInfos[i].Name, "WAVE.bin")) == false) continue;

                    RecordManager rm = new RecordManager();
                    int result = rm.CreateFileMem(drvInfos[i].Name);
                    if (result == 0)
                    {
                        using (FormLoadRecord flr = new FormLoadRecord())
                        {
                            flr.RecordManager = rm;
                            if (flr.ShowDialog() == DialogResult.OK)
                            {
                                formList.AddRecordManager(rm);
                            }
                        }
                    }
                    return;
                }
            }
            MessageBox.Show("未查找到数据源!", "查找失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void buttonUSBCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (FormDiskSelect formSel = new FormDiskSelect())
            {
                formSel.StartPosition = FormStartPosition.CenterScreen;
                if (formSel.ShowDialog() == DialogResult.OK)
                {
                    RecordManager rm = new RecordManager();
                    int result = rm.CreateFileMem(formSel.SelectDisk);
                    if (result == 0)
                    {
                        using (FormLoadRecord flr = new FormLoadRecord())
                        {
                            flr.RecordManager = rm;
                            if (flr.ShowDialog() == DialogResult.OK)
                            {
                                formList.AddRecordManager(rm);
                            }
                        }
                    }
                }
            }
        }

        private void buttonLocalFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                for (int i = 0; i < xtraTabbedMdiManager1.Pages.Count; i++)
                    if (xtraTabbedMdiManager1.Pages[i].MdiChild is FormSerialRecord)
                    {
                        FormSerialRecord fsr = xtraTabbedMdiManager1.Pages[i].MdiChild as FormSerialRecord;
                        fsr.DeleteRecord();
                        fsr.Dispose();
                    }
            }
            catch { }

            while (xtraTabbedMdiManager1.Pages.Count != 1)
            {
                xtraTabbedMdiManager1.Pages.RemoveAt(xtraTabbedMdiManager1.Pages.Count - 1);
            }
            ribbonPage2.Visible = ribbonPage3.Visible = ribbonPage4.Visible = false;
            //刷新列表->更新!?
            FormWindowState fws = WindowState;
            WindowState = (fws == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal);
            WindowState = fws;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    formList.ClearAllRecord();
                    RecordManager rm = new RecordManager();
                    int result = rm.CreateFileMem(Path.GetDirectoryName(ofd.FileName));
                    //if (result == 0)
                    {
                        using (FormLoadRecord flr = new FormLoadRecord())
                        {
                            flr.RecordManager = rm;
                            if (flr.ShowDialog() == DialogResult.OK)
                            {
                                formList.AddRecordManager(rm);
                            }
                        }
                    }
                }
            }
        }

        public void ReLoadRecord(string path)            //重载记录
        {
            try
            {
                for (int i = 0; i < xtraTabbedMdiManager1.Pages.Count; i++)
                    if (xtraTabbedMdiManager1.Pages[i].MdiChild is FormSerialRecord)
                    {
                        FormSerialRecord fsr = xtraTabbedMdiManager1.Pages[i].MdiChild as FormSerialRecord;
                        fsr.DeleteRecord();
                        fsr.Dispose();
                    }
            }
            catch { }
            //刷新列表->删除旧的数据标签
            while (xtraTabbedMdiManager1.Pages.Count != 1)
            {
                xtraTabbedMdiManager1.Pages.RemoveAt(xtraTabbedMdiManager1.Pages.Count - 1);
            }
            ribbonPage2.Visible = ribbonPage3.Visible = ribbonPage4.Visible = false;
            //刷新列表->更新!?
            FormWindowState fws = WindowState;
            WindowState = (fws == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal);
            WindowState = fws;

            if (Directory.Exists(path) == false)
            {
                MessageBox.Show("目录不存在或被移动,请重新选择!");
            }
            formList.ClearAllRecord();
            RecordManager rm = new RecordManager();
            int result = rm.CreateFileMem(path);
            //if (result == 0)
            {
                using (FormLoadRecord flr = new FormLoadRecord())
                {
                    flr.RecordManager = rm;
                    if (flr.ShowDialog() == DialogResult.OK)
                    {
                        formList.AddRecordManager(rm);
                    }
                }
            }
        }

        public void ShowRecordSerial(RecordManager rm)
        {
            for (int i = 0; i < xtraTabbedMdiManager1.Pages.Count; i++)
            {
                if (xtraTabbedMdiManager1.Pages[i].MdiChild is FormSerialRecord)
                {
                    FormSerialRecord fsr = xtraTabbedMdiManager1.Pages[i].MdiChild as FormSerialRecord;
                    if (fsr.RecordManager == rm)
                    {
                        xtraTabbedMdiManager1.SelectedPage = xtraTabbedMdiManager1.Pages[i];
                        return;
                    }
                }
            }

            FormSerialRecord formNew = new FormSerialRecord();
            formNew.RecordManager = rm;
            formNew.MdiParent = this;
            formNew.Show();
        }

        public void ShowRecordStatus(RecordManager rm)
        {
            for (int i = 0; i < xtraTabbedMdiManager1.Pages.Count; i++)
            {
                if (xtraTabbedMdiManager1.Pages[i].MdiChild is FormStatusRecord)
                {
                    FormStatusRecord fsr = xtraTabbedMdiManager1.Pages[i].MdiChild as FormStatusRecord;
                    if (fsr.RecordManager == rm)
                    {
                        xtraTabbedMdiManager1.SelectedPage = xtraTabbedMdiManager1.Pages[i];
                        return;
                    }
                }
            }

            FormStatusRecord formNew = new FormStatusRecord();
            formNew.RecordManager = rm;
            formNew.MdiParent = this;
            formNew.Show();
        }

        public void ShowRecordVoice(RecordManager rm)
        {
            for (int i = 0; i < xtraTabbedMdiManager1.Pages.Count; i++)
            {
                if (xtraTabbedMdiManager1.Pages[i].MdiChild is FormVoiceRecord)
                {
                    FormVoiceRecord fsr = xtraTabbedMdiManager1.Pages[i].MdiChild as FormVoiceRecord;
                    if (fsr.RecordManager == rm)
                    {
                        xtraTabbedMdiManager1.SelectedPage = xtraTabbedMdiManager1.Pages[i];
                        return;
                    }
                }
            }

            FormVoiceRecord formNew = new FormVoiceRecord();
            formNew.RecordManager = rm;
            formNew.MdiParent = this;
            formNew.Show();
        }
        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool state = barEditItem1.Enabled;
            barEditItem1.Enabled = !state;
            barButtonItem7.ImageIndex = state ? 1 : 0;
            if (state)
            {
                barEditItem1.EditValue = null;
            }
            else
            {
                barButtonItem8.ImageIndex = 1;
                barButtonItem9.ImageIndex = 1;
                barEditItem1.EditValue = CommonType[0];
                barEditItem2.Enabled = false;
                barEditItem2.EditValue = null;
                barEditItem3.Enabled = false;
                barEditItem3.EditValue = "16进制命令";

                barEditItem_Type.Enabled = false;
                barEditItem_Type.EditValue = "业务类型";
            }

        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool state = barEditItem2.Enabled;
            barEditItem2.Enabled = !state;
            barButtonItem8.ImageIndex = state ? 1 : 0;
            if (state)
            {
                barEditItem2.EditValue = null;
            }
            else
            {
                barButtonItem7.ImageIndex = 1;
                barButtonItem9.ImageIndex = 1;
                barEditItem2.EditValue = DetailType[0];
                barEditItem1.Enabled = false;
                barEditItem1.EditValue = null;
                barEditItem3.Enabled = false;
                barEditItem3.EditValue = "16进制命令";

                barEditItem_Type.Enabled = false;
                barEditItem_Type.EditValue = "业务类型";
            }
        }

        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool state = barEditItem3.Enabled;
            barEditItem3.Enabled = !state;
            barEditItem_Type.Enabled = !state;
            barButtonItem9.ImageIndex = state ? 1 : 0;
            if (state)
            {
                barEditItem3.EditValue = "16进制命令";

                barEditItem_Type.EditValue = "业务类型";
            }
            else
            {
                barEditItem3.EditValue = "";
                barEditItem_Type.EditValue = "";

                barButtonItem7.ImageIndex = 1;
                barButtonItem8.ImageIndex = 1;
                barEditItem1.Enabled = false;
                barEditItem2.Enabled = false;
                barEditItem1.EditValue = null;
                barEditItem2.EditValue = null;
            }
        }

        private void buttonSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)        //串口->查找数据
        {
            lastSearchAction = 0;
            if (barEditItem4.EditValue == null) return;                         //time
            if (barEditItem5.EditValue == null) return;
            if (xtraTabbedMdiManager1.SelectedPage.MdiChild is FormSerialRecord)
            {
                searchCond.Port = -1;
                searchCond.Style = -1;
                searchCond.Command = -1;
                searchCond.AllTime = (int)barButtonItem10.Tag == 0 ? true : false;      //是否启用"所有时间"
                if (barEditItem1.Enabled)               //常用类型
                {
                    string styleTmp = barEditItem1.EditValue.ToString();

                    for (int i = 0; i < CommonType.Length; i++)
                    {
                        if (CommonType[i] == styleTmp)
                        {
                            searchCond.Port = SCond1[i][0];
                            searchCond.Style = SCond1[i][1];
                            searchCond.Command = SCond1[i][2];
                            break;
                        }
                    }
                }
                //---------------------------------------------------------------------------------------
                if (barEditItem2.Enabled)               //具体类型
                {
                    string styleTmp = barEditItem2.EditValue.ToString();        //选择项

                    for (int i = 0; i < DetailType.Length - 4; i++)                 //DetailType:只读静态字符串数组("MMI选择工作模式"等)
                    {
                        if (DetailType[i] == styleTmp)
                        {
                            searchCond.Port = SCond2[i][0];
                            searchCond.Style = SCond2[i][1];
                            searchCond.Command = SCond2[i][2];
                            break;
                        }
                    }
                    for (int i = DetailType.Length - 4; i < DetailType.Length; i++)
                    {
                        if (DetailType[i] == styleTmp)
                        {
                            searchCond.Port = SCond2[i][0];
                            searchCond.Style = SCond2[i][1];
                            searchCond.Command = SCond2[i][2];
                            searchCond.my_number = SCond2[i][3];
                            break;
                        }
 
                    }
                }
                //---------------------------------------------------------------------------------------
                if (barEditItem3.Enabled)               //数字命令
                {
                    if (barEditItem3.EditValue != null)
                    {
                        try { searchCond.Style = int.Parse(barEditItem_Type.EditValue.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier); }    //BQ:业务类型选项,新增                        
                        catch (Exception) { searchCond.Style = -1; }

                        try { searchCond.Command = int.Parse(barEditItem3.EditValue.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier); }
                        catch (Exception) { searchCond.Command = -1; }
                    }
                }

                searchCond.TimeBegin = (DateTime)(barEditItem4.EditValue);
                searchCond.TimeEnd = (DateTime)(barEditItem5.EditValue);

                FormSerialRecord fsr = xtraTabbedMdiManager1.SelectedPage.MdiChild as FormSerialRecord;

                fsr.Search(searchCond);
                fsr.UpdateSidebarRS();
            }
        }

        private void buttonExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (xtraTabbedMdiManager1.SelectedPage.MdiChild is FormSerialRecord)
            {
                FormSerialRecord fsr = xtraTabbedMdiManager1.SelectedPage.MdiChild as FormSerialRecord;
                fsr.ExportExcel();
            }
        }

        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int tag = (int)barButtonItem10.Tag;
            barButtonItem10.Tag = 1 - tag;
            barButtonItem10.ImageIndex = 1 - tag;
            if (tag == 1)
            {
                if (xtraTabbedMdiManager1.SelectedPage.MdiChild is FormSerialRecord)
                {
                    FormSerialRecord fsr = xtraTabbedMdiManager1.SelectedPage.MdiChild as FormSerialRecord;
                    barEditItem4.EditValue = fsr.RecordManager.StatusBeginTime;
                    barEditItem5.EditValue = fsr.RecordManager.StatusEndTime;
                }
            }
        }

        private void buttonSearchStatus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barEditItem6.EditValue == null) return;
            if (barEditItem7.EditValue == null) return;
            if (xtraTabbedMdiManager1.SelectedPage.MdiChild is FormStatusRecord)
            {

                searchStatusCond.Port = -1;
                searchStatusCond.Style = -1;
                searchStatusCond.Command = -1;
                searchStatusCond.AllTime = (int)barButtonItem3.Tag == 0 ? true : false;

                searchStatusCond.TimeBegin = (DateTime)(barEditItem6.EditValue);
                searchStatusCond.TimeEnd = (DateTime)(barEditItem7.EditValue);


                FormStatusRecord fsr = xtraTabbedMdiManager1.SelectedPage.MdiChild as FormStatusRecord;

                fsr.Search(searchStatusCond);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int tag = (int)barButtonItem3.Tag;
            barButtonItem3.Tag = 1 - tag;
            barButtonItem3.ImageIndex = 1 - tag;
            if (tag == 1)
            {
                if (xtraTabbedMdiManager1.SelectedPage.MdiChild is FormStatusRecord)
                {
                    FormStatusRecord fsr = xtraTabbedMdiManager1.SelectedPage.MdiChild as FormStatusRecord;
                    barEditItem6.EditValue = fsr.RecordManager.StatusBeginTime;
                    barEditItem7.EditValue = fsr.RecordManager.StatusEndTime;
                }
            }
        }

        private void buttonSerial_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RecordManager rm = formList.GetFocusedRecord();
            if (rm == null)
            {
                MessageBox.Show("请选择要查看的记录!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ShowRecordSerial(rm);
            }
        }

        private void buttonStatus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RecordManager rm = formList.GetFocusedRecord();
            if (rm == null)
            {
                MessageBox.Show("请选择要查看的记录!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ShowRecordStatus(rm);
            }
        }

        private void buttonVoice_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RecordManager rm = formList.GetFocusedRecord();
            if (rm == null)
            {
                MessageBox.Show("请选择要查看的记录!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ShowRecordVoice(rm);
            }
        }

        private void buttonExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Environment.Exit(0);
        }


        private void barStaticItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barStaticItem1.ImageIndex = 2;
            barEditItem10.ImageIndex = 3;
        }

        private void barEditItem10_ShowingEditor(object sender, DevExpress.XtraBars.ItemCancelEventArgs e)
        {
            barStaticItem1.ImageIndex = 3;
            barEditItem10.ImageIndex = 2;
        }

        private void barStaticItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barStaticItem2.ImageIndex = 2;
            barStaticItem3.ImageIndex = 3;
        }

        private void barStaticItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barStaticItem2.ImageIndex = 3;
            barStaticItem3.ImageIndex = 2;
        }

        private void buttonSearchVoice_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barEditItem8.EditValue == null) return;
            if (barEditItem9.EditValue == null) return;
            if (xtraTabbedMdiManager1.SelectedPage.MdiChild is FormVoiceRecord)
            {

                searchVoiceCond.Port = -1;
                searchVoiceCond.Style = -1;
                searchVoiceCond.Command = -1;

                //searchVoiceCond.AllTime = false;
                searchVoiceCond.TimeBegin = (DateTime)(barEditItem8.EditValue);
                searchVoiceCond.TimeEnd = (DateTime)(barEditItem9.EditValue);


                FormVoiceRecord fsr = xtraTabbedMdiManager1.SelectedPage.MdiChild as FormVoiceRecord;

                fsr.Search(searchVoiceCond);
            }
        }

        private void buttonCreateWavFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (xtraTabbedMdiManager1.SelectedPage.MdiChild is FormVoiceRecord)
            {
                FormVoiceRecord fvr = xtraTabbedMdiManager1.SelectedPage.MdiChild as FormVoiceRecord;

                object val = barEditItem10.EditValue;

                fvr.CreateFile(new CreateCondition(barStaticItem1.ImageIndex == 2 ? true : false, barEditItem10.ImageIndex == 2 ? true : false, val == null ? "" : val.ToString()));
            }

        }

        private void ribbonControl1_SelectedPageChanged(object sender, EventArgs e)
        {
            if (ribbonControl1.SelectedPage == ribbonPage1)
            {
                dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
                xtraTabbedMdiManager1.SelectedPage = xtraTabbedMdiManager1.Pages[formList];
            }
            else if (ribbonControl1.SelectedPage == ribbonPage2)            //串行数据
            {
                dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                for (int i = 0; i < xtraTabbedMdiManager1.Pages.Count; i++)
                {
                    if (xtraTabbedMdiManager1.Pages[i].MdiChild is FormSerialRecord)
                    {
                        xtraTabbedMdiManager1.SelectedPage = xtraTabbedMdiManager1.Pages[i];
                        break;
                    }
                }
            }
            else if (ribbonControl1.SelectedPage == ribbonPage3)
            {
                dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
                for (int i = 0; i < xtraTabbedMdiManager1.Pages.Count; i++)
                {
                    if (xtraTabbedMdiManager1.Pages[i].MdiChild is FormStatusRecord)
                    {
                        xtraTabbedMdiManager1.SelectedPage = xtraTabbedMdiManager1.Pages[i];
                        break;
                    }
                }
            }
            else if (ribbonControl1.SelectedPage == ribbonPage4)
            {
                dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
                for (int i = 0; i < xtraTabbedMdiManager1.Pages.Count; i++)
                {
                    if (xtraTabbedMdiManager1.Pages[i].MdiChild is FormVoiceRecord)
                    {
                        xtraTabbedMdiManager1.SelectedPage = xtraTabbedMdiManager1.Pages[i];
                        break;
                    }
                }
            }

        }

        private void buttonExportStatus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            if (xtraTabbedMdiManager1.SelectedPage.MdiChild is FormStatusRecord)
            {
                FormStatusRecord fsr = xtraTabbedMdiManager1.SelectedPage.MdiChild as FormStatusRecord;
                fsr.ExportExcel();
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //ribbonPage2.Visible = ribbonPage3.Visible = ribbonPage4.Visible = true;
            Process currentProcess = Process.GetCurrentProcess(); //获取当前进程   
            string zz = currentProcess.ProcessName;
            System.Diagnostics.Process[] myProcesses = System.Diagnostics.Process.GetProcessesByName(zz);//获取指定的进程名   
            if (myProcesses.Length > 1) //如果可以获取到知道的进程名则说明已经启动
            {
                MessageBox.Show("程序已启动！");
                Application.Exit();              //关闭系统
            }          
        }

        private void barButtonMultiSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            lastSearchAction = 1;
            if (mts == null) mts = new MultiSearch();
            mts.Owner = this;
            mts.SearchPage = 0;
            mts.ShowDialog();

            FormSerialRecord fsr = xtraTabbedMdiManager1.SelectedPage.MdiChild as FormSerialRecord;

            if (mts.SearchPage == 1)                        //search type 1
            {
                List<SearchCondition> searchCondList = new List<SearchCondition>();
                mts.GetSearchCondList(ref searchCondList);

                DateTime timeBegin = (DateTime)(barEditItem4.EditValue);
                DateTime timeEnd = (DateTime)(barEditItem5.EditValue);
                bool isAllTime = barButtonItem10.ImageIndex == 0 ? true : false;        //是否勾选"全时间"

                fsr.SearchAppend(searchCondList, timeBegin, timeEnd, isAllTime);
            }
            else if (mts.SearchPage == 2)                    //search type 2:Adv搜索
            {
                DateTime timeBegin = (DateTime)(barEditItem4.EditValue);
                DateTime timeEnd = (DateTime)(barEditItem5.EditValue);
                bool isAllTime = barButtonItem10.ImageIndex == 0 ? true : false;        //是否勾选"全时间"

                List<int> src_port_list = new List<int>();
                List<int> dst_port_list = new List<int>();
                List<int> type_list = new List<int>();
                List<int> commmand_list = new List<int>();

                src_port_list = mts.srcPortInt;
                dst_port_list = mts.dstPortInt;
                type_list = mts.TypeInt;
                commmand_list = mts.CommandInt;

                fsr.SearchDetail(timeBegin, timeEnd, isAllTime, src_port_list, dst_port_list, type_list, commmand_list);
                fsr.UpdateSidebarRS();
            }
        }

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)     //播放文件
        {
            FormVoiceRecord fvr = xtraTabbedMdiManager1.SelectedPage.MdiChild as FormVoiceRecord;
            fvr.playwav();
        }

        public void ShowSideInfo()                              //显示侧边栏信息
        {
            if (sidebarRS != null)
            {
                this.textBox1.Text = "\r\n" + sidebarRS.ExplainInfo;
                this.textBox2.Text = sidebarRS.OriginData;
                this.textBox3.Text = sidebarRS.RecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                this.textBox4.Text = sidebarRS.SrcPortName2.Replace('—', ':');
                this.textBox5.Text = sidebarRS.SrcAddress;
                this.textBox6.Text = sidebarRS.DstPortName2.Replace('—', ':');
                this.textBox7.Text = sidebarRS.DstAddress;
                this.textBox8.Text = sidebarRS.RecordType.ToString("X2");
                this.textBox9.Text = sidebarRS.Command.ToString("X2");
            }
            else
            {
                textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text
                              = textBox6.Text = textBox7.Text = textBox8.Text = textBox9.Text = "";
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
        }

        //单击侧边栏文本框，高亮原始数据对应字节
        private void textBox4_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Select();
                textBox2.Select(12, 2);
            }
            catch { }
        }
        private void textBox5_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Select();
                textBox2.Select(15, 2);
                int addLength = Convert.ToInt32(textBox2.SelectedText, 16);
                textBox2.Select(18, addLength * 3 - 1);
            }
            catch { }
        }
        private void textBox6_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Select();
                textBox2.Select(15, 2);
                int addLength = Convert.ToInt32(textBox2.SelectedText, 16);
                textBox2.Select((6 + addLength) * 3, 2);
            }
            catch { }
        }
        private void textBox7_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Select();
                int addLen1 = Convert.ToInt32(textBox2.Text.Substring(15, 2), 16);
                int addLen2 = Convert.ToInt32(textBox2.Text.Substring((addLen1 + 7) * 3, 2), 16);
                textBox2.Select((8 + addLen1) * 3, addLen2 * 3 - 1);
            }
            catch { }
        }
        private void textBox8_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Select();
                int addLen1 = Convert.ToInt32(textBox2.Text.Substring(15, 2), 16);
                int addLen2 = Convert.ToInt32(textBox2.Text.Substring((addLen1 + 7) * 3, 2), 16);
                textBox2.Select((8 + addLen1 + addLen2) * 3, 2);
            }
            catch { }
        }
        private void textBox9_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Select();
                int addLen1 = Convert.ToInt32(textBox2.Text.Substring(15, 2), 16);
                int addLen2 = Convert.ToInt32(textBox2.Text.Substring((addLen1 + 7) * 3, 2), 16);
                textBox2.Select((9 + addLen1 + addLen2) * 3, 2);
            }
            catch { }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == string.Empty) return;
            try
            {
                int lineNum = textBox1.GetLineFromCharIndex(textBox1.SelectionStart);
                int addLen1 = Convert.ToInt32(textBox2.Text.Substring(15, 2), 16);
                int addLen2 = Convert.ToInt32(textBox2.Text.Substring((addLen1 + 7) * 3, 2), 16);
                int dataHead = addLen1 + addLen2 + 10;
                Point highLightPoint = HighLightDataGram.CaretToBytes(sidebarRS.OriginData, lineNum);
                textBox2.Select();
                textBox2.Select((dataHead + highLightPoint.X - 1) * 3, highLightPoint.Y * 3 - 1);
            }
            catch { }
        }

        private void barButtonItem_TimeFilter(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            searchWithTime swt;
            if (lastSearchAction == 0)
            {
                swt = new searchWithTime(buttonSearch_ItemClick);
                swt(sender, e);
            }
            else if (lastSearchAction == 1)                 //重复...
            {
                FormSerialRecord fsr = xtraTabbedMdiManager1.SelectedPage.MdiChild as FormSerialRecord;

                if (mts.SearchPage == 1)                        //search type 1
                {
                    List<SearchCondition> searchCondList = new List<SearchCondition>();
                    mts.GetSearchCondList(ref searchCondList);

                    DateTime timeBegin = (DateTime)(barEditItem4.EditValue);
                    DateTime timeEnd = (DateTime)(barEditItem5.EditValue);
                    bool isAllTime = barButtonItem10.ImageIndex == 0 ? true : false;
                    fsr.SearchAppend(searchCondList, timeBegin, timeEnd, isAllTime);
                }
                else if (mts.SearchPage == 2)
                {
                    DateTime timeBegin = (DateTime)(barEditItem4.EditValue);
                    DateTime timeEnd = (DateTime)(barEditItem5.EditValue);
                    bool isAllTime = barButtonItem10.ImageIndex == 0 ? true : false;

                    List<int> src_port_list = new List<int>();
                    List<int> dst_port_list = new List<int>();
                    List<int> type_list = new List<int>();
                    List<int> commmand_list = new List<int>();

                    src_port_list = mts.srcPortInt;
                    dst_port_list = mts.dstPortInt;
                    type_list = mts.TypeInt;
                    commmand_list = mts.CommandInt;

                    fsr.SearchDetail(timeBegin, timeEnd, isAllTime, src_port_list, dst_port_list, type_list, commmand_list);

                    fsr.UpdateSidebarRS();
                }
            }
        }

        private void barButton_关于(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void barButton_帮助(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string filePath = @"帮助文档.pdf";
            if (filePath.Equals(""))
            {
                MessageBox.Show("路径不能为空！", "操作提示");
                return;
            }
            //先判断文件是否存在，不存在则提示   
            if (!System.IO.File.Exists(filePath))
            {
                MessageBox.Show("指定文件不存在！", "操作提示");
                return;
            }
            //存在则打开   
            //System.Diagnostics.Process.Start("explorer.exe", filePath);  
            System.Diagnostics.ProcessStartInfo Info = new System.Diagnostics.ProcessStartInfo(filePath);
            System.Diagnostics.Process Pro = System.Diagnostics.Process.Start(Info);
        }

        private void 格式化工具(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult MsgBoxResult;                                                   //设置对话框的返回值
            string WarningInfo = "        即将打开的是第三方提供的低级格式化工具，该工具使用不当可能会对磁盘造成损害。\r\n        请确保您已经阅读《用户手册》中关于低级格式化工具的使用方法并了解其可能带来的后果。\r\n        如果您还不了解相关内容，请点击\"否\"\r\n        是否继续打开格式化工具？";
            MsgBoxResult = MessageBox.Show(WarningInfo,      //对话框的显示内容
                                                                 "警告！",                              //对话框的标题
            MessageBoxButtons.YesNo,                                                  //定义对话框的按钮，这里定义了YSE和NO两个按钮
            MessageBoxIcon.Exclamation,                                               //定义对话框内的图表式样，这里是一个黄色三角型内加一个感叹号
            MessageBoxDefaultButton.Button2);                                        //定义对话框的按钮式样
            if (MsgBoxResult == DialogResult.Yes)                                    //如果对话框的返回值是YES（按"Y"按钮）
            {
                try
                {
                    System.Diagnostics.Process.Start(Path.Combine(Application.StartupPath, "HDD Low Level Format Tool4.25.exe"));
                }
                catch (Exception e975) { MessageBox.Show(e975.ToString()); }
            }
            else if (MsgBoxResult == DialogResult.No)                                    //如果对话框的返回值是NO（按"N"按钮）
            {
                return;
            }
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            searchVoiceCond.AllTime = !searchVoiceCond.AllTime;
            barButtonItem6.ImageIndex = searchVoiceCond.AllTime ? 0 : 1;
        }



    }
}