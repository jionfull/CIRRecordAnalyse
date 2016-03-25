using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CIRRecordAnalyse
{
    public partial class WaittingForm_Exclusive : Form
    {
        public WaittingForm_Exclusive()
        {
            InitializeComponent();
        }

        private void WaittingForm_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value += progressBar1.Value == 100 ? (-98) : 2;
        }
    }
}
