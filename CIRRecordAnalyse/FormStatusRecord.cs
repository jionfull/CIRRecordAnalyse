using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CIRRecordAnalyse.Core;

namespace CIRRecordAnalyse
{
    public partial class FormStatusRecord : Form
    {

        RecordManager rm;
        DateTime searchBeginTime = DateTime.Now;
        DateTime searchEndTime = DateTime.Now;
        BindingList<RecordStatus> listBind = new BindingList<RecordStatus>();

        public FormStatusRecord()
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

        public DateTime SearchBeginTime
        {
            get { return searchBeginTime; }
        }

        public DateTime SearchEndTime
        {
            get { return searchEndTime; }
        }

        public void Search(SearchCondition cond)
        {
            searchBeginTime = cond.TimeBegin;
            searchEndTime = cond.TimeEnd;
            gridControl1.DataSource = null;
            listBind.Clear();
            rm.SearchRecord(cond, listBind);
            gridControl1.DataSource = listBind;
        }

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
                            formExport.ListStatusRecord = listBind;
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

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                if (e.Column.FieldName == "GPSData")
                {
                    if (e.DisplayText == "故障")
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
                else if (e.Column.FieldName == "MainUnit")
                {
                    if (e.DisplayText == "故障")
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
                else if (e.Column.FieldName == "BatteryStatus")
                {
                    if (e.DisplayText == "故障")
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
        }
    }
}
