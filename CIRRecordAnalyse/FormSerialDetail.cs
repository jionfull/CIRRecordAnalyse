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
    public partial class FormSerialDetail : Form
    {

        RecordSerial rs;
        public FormSerialDetail()
        {
            InitializeComponent();
        }



        public RecordSerial Record
        {
            set
            {
                if (value == null) return;
                rs = value;
                this.textBox1.Text = rs.RecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                this.textBox2.Text = rs.SrcPortName2;
                this.textBox3.Text = rs.SrcAddress;
                this.textBox4.Text = rs.DstPortName2;
                this.textBox5.Text = rs.DstAddress;
                this.textBox6.Text = rs.RecordType.ToString("X2");
                this.textBox7.Text = rs.Command.ToString("X2");
                this.textBox9.Text = rs.ExplainInfo;
                this.textBox10.Text = rs.OriginData;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
