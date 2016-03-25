using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CIRRecordAnalyse
{
    public partial class FormDiskSelect : Form
    {
        public FormDiskSelect()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);



            DriveInfo[] drvInfos = DriveInfo.GetDrives();
            for (int i = 0; i < drvInfos.Length; i++)
            {
                if (drvInfos[i].IsReady && (drvInfos[i].DriveFormat == "FAT" || drvInfos[i].DriveFormat == "FAT32") && drvInfos[i].DriveType == DriveType.Removable)
                {
                    if (File.Exists(Path.Combine(drvInfos[i].Name, "MENU.bin")) == false) continue;
                    if (File.Exists(Path.Combine(drvInfos[i].Name, "DATA.bin")) == false) continue;
                    if (File.Exists(Path.Combine(drvInfos[i].Name, "WAVE.bin")) == false) continue;
                    comboBox1.Items.Add(drvInfos[i].Name);
                }
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
                button1.Enabled = true;
            }
        }

        string selectDisk = "";
        public string SelectDisk
        {
            get
            {
                return selectDisk;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                selectDisk = comboBox1.Text;
                DialogResult = DialogResult.OK;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            DialogResult = DialogResult.Cancel;
        }
    }
}
