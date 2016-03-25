using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CIRRecordAnalyse
{
    public partial class FormPreload : Form
    {
        public FormPreload()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.DialogResult = DialogResult.OK;
        }
    }
}
