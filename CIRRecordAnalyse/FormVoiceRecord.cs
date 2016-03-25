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

namespace CIRRecordAnalyse
{
    public partial class FormVoiceRecord : Form
    {
        RecordManager rm;
        DateTime searchBeginTime = DateTime.Now;
        DateTime searchEndTime = DateTime.Now;
        BindingList<RecordVoice> listBind = new BindingList<RecordVoice>();
        //
        RecordVoice rvPlaying;


        public FormVoiceRecord()
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

        public void CreateFile(CreateCondition cond)                                              //生成波形文件
        {
            if (cond.SelectFile)                      //如果是输出单文件...
            {
                int rowHandle = gridView1.FocusedRowHandle;             //获取焦点记录

                if (rowHandle >= 0)
                {
                    RecordVoice rv = gridView1.GetRow(rowHandle) as RecordVoice;

                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "波形文件(*.wav)|*.wav";
                        sfd.FileName = rv.RecordTime.ToString("yyyy-MM-dd HH时mm分ss秒");
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            if (rv.CreateWaveFile(sfd.FileName))
                            {
                                MessageBox.Show("波形文件生成完毕!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请选择要转换的记录!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }


            else                                                    //批量输出.....
            {
                string[] seg = cond.IndexText.Split(new char[] { '-' });
                if (seg.Length == 2)
                {
                    try
                    {
                        int startIndex = int.Parse(seg[0]);
                        int endIndex = int.Parse(seg[1]);

                        if (startIndex > endIndex)
                        {
                            int tempswap = startIndex;
                            startIndex = endIndex;
                            endIndex = tempswap;
                        }

                        if (startIndex < 1 || endIndex > gridView1.RowCount)
                        {
                            MessageBox.Show("选择的记录超出边界!");
                            return;
                        }

                        using (SaveFileDialog sfd2 = new SaveFileDialog())
                        {
                            sfd2.Filter = "波形文件(*.wav)|*.wav";
                            sfd2.FileName = "文件名无需指定";
                            if (sfd2.ShowDialog() == DialogResult.OK)
                            {
                                string strpath = Path.GetDirectoryName(sfd2.FileName);
                                for (int i = startIndex; i <= endIndex; i++)
                                {
                                    RecordVoice rvs = gridView1.GetRow(i - 1) as RecordVoice;
                                    string strFileName = strpath + "\\" + rvs.RecordTime.ToString("yyyy-MM-dd HH时mm分ss秒" + ".wav");
                                    rvs.CreateWaveFile(strFileName);
                                }
                                MessageBox.Show("波形文件批量生成完毕!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            /*          else{}
                        {                
                            string[] seg = cond.IndexText.Split(new char[] { '-' });
                            if (seg.Length == 2)
                            {
                                try
                                {
                                    int startIndex = int.Parse(seg[0]);
                                    int endIndex = int.Parse(seg[1]);
                                }
                                catch (Exception){}
                            }
                        }*/
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = gridView1.CalcHitInfo(e.Location);
                if (hitInfo.InRow)
                {
                    RecordVoice rv = gridView1.GetRow(hitInfo.RowHandle) as RecordVoice;
                    if (rv != null)
                    {
                        string dir = Path.Combine(Application.StartupPath, "Temp");
                        if (Directory.Exists(dir) == false)
                        {
                            Directory.CreateDirectory(dir);
                        }
                        string fileName = Path.Combine(dir, rv.RecordTime.ToString("yyyy-MM-dd HH!mm!ss.wav"));


                        string[] wavFiles = Directory.GetFiles(dir, "*.wav");

                        try
                        {
                            foreach (string fileWave in wavFiles)
                            {
                                if (fileName != fileWave)
                                {
                                    File.Delete(fileWave);
                                }
                            }
                        }
                        catch { }

                        //双击->播放文件
                        if (rv != rvPlaying)
                        {
                            if (rv.CreateWaveFile(fileName))
                            {
                                try
                                {
                                    Process.Start(Path.Combine(Application.StartupPath, "AIRPLAY.exe"), "\"" + fileName + "\"");
                                    rvPlaying = rv;
                                }
                                catch (Exception e257) { MessageBox.Show(e257.ToString()); }
                            }
                        }
                        else MessageBox.Show("波形文件正在播放!");
                        //
                    }
                }
            }
        }

        public void playwav()
        {

            int rowHandle = gridView1.FocusedRowHandle;             //获取焦点记录

            if (rowHandle >= 0)
            {
                RecordVoice rv = gridView1.GetRow(rowHandle) as RecordVoice;
                if (rv != null)
                {
                    string dir = Path.Combine(Application.StartupPath, "Temp");
                    if (Directory.Exists(dir) == false)
                    {
                        Directory.CreateDirectory(dir);
                    }
                    string fileName = Path.Combine(dir, rv.RecordTime.ToString("yyyy-MM-dd HH!mm!ss.wav"));

                    string[] wavFiles = Directory.GetFiles(dir, "*.wav");

                    try
                    {
                        foreach (string fileWave in wavFiles)
                        {
                            if (fileName != fileWave)
                            {
                                File.Delete(fileWave);
                            }
                        }
                    }
                    catch { }

                    if (rv != rvPlaying)
                    {
                        if (rv.CreateWaveFile(fileName))
                        {
                            try
                            {
                                Process.Start(Path.Combine(Application.StartupPath, "AIRPLAY.exe"), "\"" + fileName + "\"");
                                rvPlaying = rv;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                        }
                    }
                    else MessageBox.Show("波形文件正在播放!");
                }
            }
        }
    }
}
