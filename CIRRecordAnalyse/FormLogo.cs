using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using System.Reflection;
using System.Threading;

namespace CIRRecordAnalyse
{
    public partial class FormLogo : Form
    {
        public FormLogo()
        {
            InitializeComponent();
        }


        private void ProcLoad()
        {

           // Thread.Sleep(500);

            this.Invoke(new MethodInvoker(delegate {
                
                using (FormPreload form = new FormPreload())
                {
                    form.ShowDialog();
                }
                
              
            }));
            Thread.Sleep(500);
            this.Invoke(new MethodInvoker(delegate
            {

                this.DialogResult = DialogResult.OK;


            }));
           
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Thread thread = new Thread(new ThreadStart(ProcLoad));
            thread.IsBackground = true;
            thread.Start();
        }







       

    }
}
