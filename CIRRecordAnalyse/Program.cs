using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;

namespace CIRRecordAnalyse
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.Skins.SkinManager.EnableFormSkins();
            UserLookAndFeel.Default.SetSkinStyle("Seven Classic");
            using (FormLogo logo = new FormLogo())
            {
                if (logo.ShowDialog() != DialogResult.OK) return;
            }
            Application.Run(new FormMain());

           //Application.Run(new FormExportProcess());
        }
    }
}
