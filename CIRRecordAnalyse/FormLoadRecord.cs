using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CIRRecordAnalyse.Core;
using System.Threading;

namespace CIRRecordAnalyse
{
    public partial class FormLoadRecord : Form
    {
        RecordManager rm;
        Thread threadParse;
        public bool isCancel = false;

        public FormLoadRecord()
        {
            InitializeComponent();
        }

        public RecordManager RecordManager
        {
            set
            {
                rm = value;
                rm.DataParseEvent += new EventHandler<DataParseArgs>(DataParseEvent);
            }
        }

        void DataParseEvent(object sender, DataParseArgs e)
        {
            try
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    if (e.EventType == 1)
                    {
                        progressBar1.Value = (int)(e.Position % 100);
                        label2.Text = string.Format("解析文件({0}%)", (int)(e.Position % 101));
                    }
                    else if (e.EventType == 2)
                    {
                        label2.Text = "记录排序...";
                    }
                }));
            }
            catch
            {}
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            threadParse = new Thread(new ThreadStart(ProcParse));
            threadParse.IsBackground = true;
            threadParse.Start();
        }

        private void ProcParse()
        {
            if (rm != null && rm.IsLoaded)
            {
                rm.ParseFile();
                this.Invoke(new MethodInvoker(delegate
                {
                    this.DialogResult = DialogResult.OK;
                }));
            }
        }

        private void simpleButton1_Cancel(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否停止载入数据？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                isCancel = true;
                threadParse.Abort();
                Close();
            }
        }

        private void FormLoadRecord_Load(object sender, EventArgs e)
        {

        }
    }
}