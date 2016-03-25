using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CIRRecordAnalyse.Core;
using System.Runtime.InteropServices;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using System.IO;

namespace CIRRecordAnalyse
{
    public partial class FormSerialRecord : Form
    {
        RecordManager rm;
        DateTime searchBeginTime = DateTime.Now;
        DateTime searchEndTime = DateTime.Now;
        BindingList<RecordSerial> listBind = new BindingList<RecordSerial>();
        //FormSerialDetail frmDetail;

        const int SW_SHOWNOACTIVATE = 4;
        [DllImport("User32.dll ", CharSet = CharSet.Auto)]
        private static extern int ShowWindow(IntPtr hWnd, short cmdShow);

        public FormSerialRecord()
        {
            InitializeComponent();
        }

        public RecordManager RecordManager
        {
            get
            {
                return rm;
            }
            set
            {
                rm = value;
                searchBeginTime = rm.StatusBeginTime;
                searchEndTime = rm.StatusEndTime;
            }
        }

        //删除内存?
        public void DeleteRecord()
        {
            listBind.Clear();
            this.rm.Dispose();
            this.rm = null;
            GC.Collect();
        }

        public DateTime SearchBeginTime
        {
            get { return searchBeginTime; }
        }

        public DateTime SearchEndTime
        {
            get { return searchEndTime; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public void Search(SearchCondition cond)
        {
            searchBeginTime = cond.TimeBegin;
            searchEndTime = cond.TimeEnd;
            gridControl1.DataSource = null;
            listBind.Clear();
            rm.SearchRecord(cond, listBind);
            gridControl1.DataSource = listBind;
            if (listBind.Count == 0) MessageBox.Show("记录为空!");
        }
        //-----------------------------------------------
        public void SearchAppend(List<SearchCondition> cond, DateTime timeBegin, DateTime timeEnd, bool isAllTime)
        {
            searchBeginTime = timeBegin;
            searchEndTime = timeEnd;
            gridControl1.DataSource = null;
            listBind.Clear();
            rm.SearchRecord(cond, listBind, isAllTime, timeBegin, timeEnd);
            gridControl1.DataSource = listBind;
            if (listBind.Count == 0) MessageBox.Show("记录为空!");
        }
        public void SearchDetail(DateTime timeBegin, DateTime timeEnd, bool isAllTime, List<int> srcPortList, List<int> dstPortList, List<int> typeList,List<int>comList)      //按端口-业务类型查找
        {
            gridControl1.DataSource = null;
            listBind.Clear();
            rm.SearchRecordDetail(srcPortList, dstPortList, typeList,comList, timeBegin, timeEnd, isAllTime, listBind);
            gridControl1.DataSource = listBind;
            if (listBind.Count == 0) MessageBox.Show("记录为空!");
        }
        //-----------------------------------------------

        public void ExportExcel()
        {
            if (listBind.Count > 0)
            {
                if (listBind.Count > 500000)
                {
                    MessageBox.Show("超出最多50万行数据限制!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel 97-2003 工作簿(*.xls)|*.xls";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (FormExportProcess formExport = new FormExportProcess())
                        {
                            formExport.StartPosition = FormStartPosition.CenterScreen;
                            formExport.ListSerialRecord = listBind;
                            formExport.ExportFileName = sfd.FileName;
                            if (formExport.ShowDialog() == DialogResult.OK)
                            {
                                MessageBox.Show("导出数据到Excel文件完毕!");
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("没有记录数据!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridControl1_MouseDoubleClick(object sender, MouseEventArgs e)     //双击显示具体信息，功能暂取消
        {
            /*if (e.Button == MouseButtons.Left)
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = gridView1.CalcHitInfo(e.Location);
                if (hitInfo.InRow)
                {
                    RecordSerial rs = gridView1.GetRow(hitInfo.RowHandle) as RecordSerial;
                    if (frmDetail == null || frmDetail.IsDisposed)          //显示详情窗口
                    {
                        frmDetail = new FormSerialDetail();
                        frmDetail.StartPosition = FormStartPosition.Manual;
                        frmDetail.ShowInTaskbar = false;
                        frmDetail.Owner = this;
                        frmDetail.Record = rs;

                        //if (FormMain.ActiveForm != null)      //双击显示侧边栏信息
                        //{
                        //    FormMain fm = (FormMain)FormMain.ActiveForm;
                        //    bool hasData = true;
                        //    try
                        //    {
                        //        fm.ShowDetailInfo = rs.ExplainInfo;
                        //        fm.ShowOriginData = rs.OriginData;
                        //    }
                        //    catch
                        //    {
                        //        hasData = false;
                        //        //MessageBox.Show(e1.ToString());
                        //    }
                        //    fm.ShowSideInfo(hasData);
                        //}

                        frmDetail.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - frmDetail.Width - 30, Screen.PrimaryScreen.WorkingArea.Bottom - frmDetail.Height - 35);
                        ShowWindow(frmDetail.Handle, SW_SHOWNOACTIVATE);
                    }
                    else
                    {
                        frmDetail.Record = rs;
                    }
                }
            }*/
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            RecordSerial rs = gridView1.GetRow(e.FocusedRowHandle) as RecordSerial;
            if (FormMain.ActiveForm != null)                //侧边信息
            {
                FormMain fm = (FormMain)FormMain.ActiveForm;
                try
                {
                    fm.SidebarRS = rs;
                }
                catch { }
                fm.ShowSideInfo();
            }
            //if (frmDetail != null && frmDetail.IsDisposed == false)             //窗口信息
            //{
            //    frmDetail.Record = rs;
            //}
        }

        public void UpdateSidebarRS()                                   //重新搜索后更新侧栏信息
        {
            RecordSerial rs = gridView1.GetRow(0) as RecordSerial;
            if (FormMain.ActiveForm != null)
            {
                FormMain fm = (FormMain)FormMain.ActiveForm;
                try
                {
                    fm.SidebarRS = rs;
                }
                catch { }
                fm.ShowSideInfo();
            }
        }
    }
}