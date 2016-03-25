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
    public partial class FormRecordList : Form
    {
        BindingList<RecordManager> listRM = new BindingList<RecordManager>();
        BindingList<ListInfo> listInfo = new BindingList<ListInfo>();

        public FormRecordList()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //gridControl1.DataSource = listRM;
            gridControl1.DataSource = listInfo;
        }

        public void AddRecordManager(RecordManager rm)
        {
            listRM.Add(rm);
            ListInfo newListItem = new ListInfo();
            newListItem.AddNewRecordInfo(rm);
            int repeatedIndex = -1;
            for (int i = 0; i < listInfo.Count; i++)
            {
                listInfo[i].IsLoaded = false;
                if (listInfo[i].FullPath == rm.FullPath) repeatedIndex = i;
            }
            if (repeatedIndex != -1)
            {
                listInfo.Remove(listInfo[repeatedIndex]);   //载入重复的,先删除原来的
                listInfo.Insert(repeatedIndex, newListItem);
                gridView1.FocusedRowHandle = repeatedIndex;
            }
            else
            {
                listInfo.Add(newListItem);
                gridView1.FocusedRowHandle = listInfo.Count - 1;
            }
        }

        public void LoadCancel()
        {
            ClearAllRecord();
            foreach (ListInfo li in listInfo)
            {
                li.IsLoaded=false;
            }
        }

        public void ClearAllRecord()
        {
            for (int i = 0; i < listRM.Count; i++)
            {
                listRM[i].Dispose();
                listRM[i] = null;
            }
            listRM.Clear();
            GC.Collect();
        }

        private void gridControl1_MouseDoubleClick(object sender, MouseEventArgs e)             //双击->读取数据文件
        {
            if (e.Button == MouseButtons.Left)
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = gridView1.CalcHitInfo(e.Location);
                if (info.InRow)
                {                   
                    //RecordManager rm = gridView1.GetRow(info.RowHandle) as RecordManager;

                     ListInfo li = gridView1.GetRow(info.RowHandle) as ListInfo;
                     if (li.UsingState == "使用中")
                     {
                         RecordManager rm = listRM[0];
                         if (rm != null)
                         {
                             FormMain formMain = this.MdiParent as FormMain;
                             formMain.ShowRecordSerial(rm);
                         }
                     }
                     else
                     {
                         FormMain formMain = this.MdiParent as FormMain;
                         formMain.ReLoadRecord(li.FullPath);
                     }
                }
            }
        }

        private void gridControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = gridView1.CalcHitInfo(e.Location);
                if (hitInfo.InRow)
                {
                    barButtonItem1.Tag = hitInfo.RowHandle;
                    barButtonItem2.Tag = hitInfo.RowHandle;
                    barButtonItem3.Tag = hitInfo.RowHandle;
                    barButtonItem4.Tag = hitInfo.RowHandle;
                    barButtonItem5.Tag = hitInfo.RowHandle;

                    //
                    ListInfo li = gridView1.GetRow(hitInfo.RowHandle) as ListInfo;
                    if (li.UsingState == "使用中")
                        popupMenu1.ShowPopup(this.PointToScreen(e.Location));
                    else
                        popupMenu2.ShowPopup(this.PointToScreen(e.Location));
                }
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //串行数据分析
            int rowHandle = (int)(barButtonItem1.Tag);
            ListInfo li = gridView1.GetRow(rowHandle) as ListInfo;
            if (li.UsingState == "使用中")
            {
                RecordManager rm = listRM[0];
                if (rm != null)
                {
                    FormMain formMain = this.MdiParent as FormMain;
                    formMain.ShowRecordSerial(rm);
                }
            }
            else
            {
                MessageBox.Show("请先载入数据!");
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //状态数据分析
            int rowHandle = (int)(barButtonItem2.Tag);
            ListInfo li = gridView1.GetRow(rowHandle) as ListInfo;
            if (li.UsingState == "使用中")
            {
                RecordManager rm = listRM[0];
                if (rm != null)
                {
                    FormMain formMain = this.MdiParent as FormMain;
                    formMain.ShowRecordStatus(rm);
                }
            }
            else
            {
                MessageBox.Show("请先载入数据!");
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //波形数据分析
            int rowHandle = (int)(barButtonItem3.Tag);

            ListInfo li = gridView1.GetRow(rowHandle) as ListInfo;
            if (li.UsingState == "使用中")
            {
                RecordManager rm = listRM[0];
                if (rm != null)
                {
                    FormMain formMain = this.MdiParent as FormMain;
                    formMain.ShowRecordVoice(rm);
                }
            }
            else
            {
                MessageBox.Show("请先载入数据!");
            }
        }

        public RecordManager GetFocusedRecord()
        {
            if (gridView1.FocusedRowHandle >= 0)
            {
                //return (RecordManager)gridView1.GetRow(gridView1.FocusedRowHandle);
                ListInfo li = (ListInfo)gridView1.GetRow(gridView1.FocusedRowHandle);
                if (li.UsingState == "使用中")
                    return listRM[0];
                else return null;
            }
            return null;
        }

        private void Popup_reload(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //重载
            int rowHandle = (int)(barButtonItem4.Tag);

            ListInfo li = gridView1.GetRow(rowHandle) as ListInfo;
            FormMain formMain = this.MdiParent as FormMain;
            formMain.ReLoadRecord(li.FullPath);
        }

        private void Popup_delete(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int rowHandle = (int)(barButtonItem5.Tag);
            ListInfo li = gridView1.GetRow(rowHandle) as ListInfo;
            listInfo.Remove(li);
        }
    }
}
