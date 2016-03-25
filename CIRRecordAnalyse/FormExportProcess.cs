using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CIRRecordAnalyse.Core;
using System.Threading;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using System.IO;

namespace CIRRecordAnalyse
{
    public partial class FormExportProcess : Form
    {
        const int CountPerSheet = 50000;

        Thread threadExport;
        BindingList<RecordSerial> listSerialRecord = null;
        BindingList<RecordStatus> listStatusRecord = null;
        string fileName = "";

        public FormExportProcess()
        {
            InitializeComponent();

            label1.Parent = pictureBox1;
            label1.BackColor = Color.Transparent;
        }

        public string ExportFileName
        {
            set
            {
                fileName = value;
            }
        }

        public BindingList<RecordSerial> ListSerialRecord
        {
            set
            {
                this.listSerialRecord = value;
            }
        }

        public BindingList<RecordStatus> ListStatusRecord
        {
            set
            {
                this.listStatusRecord = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (listSerialRecord != null)
            {
                threadExport = new Thread(new ThreadStart(ExportSerialRecord));
                threadExport.IsBackground = true;
                threadExport.Start();
            }
            else if(listStatusRecord!=null)
            {
                threadExport = new Thread(new ThreadStart(ExportStatusRecord));
                threadExport.IsBackground = true;
                threadExport.Start();
            }

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (threadExport != null && threadExport.IsAlive)
            {
                threadExport.Abort();
            }
            this.DialogResult = DialogResult.Cancel;
        }

        private void ExportSerialRecord()
        {
            try
            {
                HSSFWorkbook hssfworkbook = CreateWorkbook();
                string[] colsName = new string[] { "时间", "源端口", "目的端口", "类型", "数据涵义","经度","纬度","原始数据" };
                int[] colsWidth = new int[] { 5000, 3000, 3000, 3000, 18000,3000,3000,18000*3 };
                HSSFSheet sheet1 = null;
                for (int k = 0; k < listSerialRecord.Count; k++)
                {


                    this.Invoke(new MethodInvoker(delegate {

                        int progress = k * progressBar1.Maximum / listSerialRecord.Count;
                        if (progress != progressBar1.Value)
                        {
                            progressBar1.Value = progress;
                        }
                    }));

          

                    if (k % CountPerSheet == 0)
                    {
                        sheet1 = hssfworkbook.CreateSheet("Sheet" + (k / CountPerSheet + 1));

                        sheet1.PrintSetup.Landscape = true;
                        sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
                        sheet1.CreateFreezePane(0, 1, 0, 1);
                        HSSFRow headerRow = sheet1.CreateRow(0);

                        for (int i = 0; i < colsName.Length; i++)
                        {
                            headerRow.CreateCell(i).SetCellValue(colsName[i]);
                            sheet1.SetColumnWidth(i, colsWidth[i]);
                        }
                    }

                    int rowIndex = k % CountPerSheet + 1;
                    HSSFRow row = sheet1.CreateRow(rowIndex);
                    row.CreateCell(0).SetCellValue(listSerialRecord[k].RecordTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    row.CreateCell(1).SetCellValue(listSerialRecord[k].SrcPortName);
                    row.CreateCell(2).SetCellValue(listSerialRecord[k].DstPortName);
                    row.CreateCell(3).SetCellValue(listSerialRecord[k].TypeName);
                    row.CreateCell(4).SetCellValue(listSerialRecord[k].DetailInfo);
                    row.CreateCell(5).SetCellValue(listSerialRecord[k].Longitude);
                    row.CreateCell(6).SetCellValue(listSerialRecord[k].Latitude);
                    row.CreateCell(7).SetCellValue(listSerialRecord[k].OriginData);
                }
                this.Invoke(new MethodInvoker(delegate
                {
                    timer1.Enabled = true;
                }));
                if (WriteToFile(fileName, hssfworkbook))
                {
                    timer1.Enabled = false;
                    MessageBox.Show("数据导出完毕!");
                }



                this.Invoke(new MethodInvoker(delegate {
                    this.DialogResult = DialogResult.OK;
                }));
            }
            catch (ThreadAbortException)
            {
 
            }
        }


        private void ExportStatusRecord()
        {
            try
            {
                HSSFWorkbook hssfworkbook = CreateWorkbook();
                string[] colsName = new string[] { "时间", "外部电源", "录音信号", "复位信号", "GPS数据", "GPS状态","主控单元数据","电池状态","电池电压"};
                int[] colsWidth = new int[] { 5000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, };
                HSSFSheet sheet1 = null;
                for (int k = 0; k < listStatusRecord.Count; k++)
                {


                    this.Invoke(new MethodInvoker(delegate
                    {

                        int progress = k * progressBar1.Maximum / listStatusRecord.Count;
                        if (progress != progressBar1.Value)
                        {
                            progressBar1.Value = progress;
                        }
                    }));



                    if (k % CountPerSheet == 0)
                    {
                        sheet1 = hssfworkbook.CreateSheet("Sheet" + (k / CountPerSheet + 1));

                        sheet1.PrintSetup.Landscape = true;
                        sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
                        sheet1.CreateFreezePane(0, 1, 0, 1);
                        HSSFRow headerRow = sheet1.CreateRow(0);

                        for (int i = 0; i < colsName.Length; i++)
                        {
                            headerRow.CreateCell(i).SetCellValue(colsName[i]);
                            sheet1.SetColumnWidth(i, colsWidth[i]);
                        }
                    }

                    int rowIndex = k % CountPerSheet + 1;
                    HSSFRow row = sheet1.CreateRow(rowIndex);
                    row.CreateCell(0).SetCellValue(listStatusRecord[k].RecordTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    row.CreateCell(1).SetCellValue(listStatusRecord[k].ExternPower);
                    row.CreateCell(2).SetCellValue(listStatusRecord[k].RecordSignal);
                    row.CreateCell(3).SetCellValue(listStatusRecord[k].ResetSignal);
                    row.CreateCell(4).SetCellValue(listStatusRecord[k].GPSData);
                    row.CreateCell(5).SetCellValue(listStatusRecord[k].GPSStatus);
                    row.CreateCell(6).SetCellValue(listStatusRecord[k].MainUnit);
                    row.CreateCell(7).SetCellValue(listStatusRecord[k].BatteryStatus);
                    row.CreateCell(8).SetCellValue(listStatusRecord[k].BatteryVoltage);
                }

                this.Invoke(new MethodInvoker(delegate
                {
                    timer1.Enabled = true;
                }));
                if (WriteToFile(fileName, hssfworkbook))
                {
                    timer1.Enabled = false;
                    MessageBox.Show("数据导出完毕!");
                }



                this.Invoke(new MethodInvoker(delegate
                {
                    this.DialogResult = DialogResult.OK;
                }));
            }
            catch (ThreadAbortException)
            {

            }
        }

        HSSFWorkbook CreateWorkbook()
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "深圳长龙";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "CIR记录文件";
            hssfworkbook.SummaryInformation = si;

            return hssfworkbook;
        }

        bool WriteToFile(string fileName, HSSFWorkbook hssfworkbook)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Create);
                hssfworkbook.Write(file);
                file.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }


        int progressIndex = 0;
        string[] progressTxt = new string[] { ".","..","...","....",".....",};
        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = "Excel文件保存中"+progressTxt[progressIndex];
            if (label1.Visible == false)
            {
                label1.Visible = true;
            }
            progressIndex = (progressIndex + 1) % 5;
        }




    }
}
