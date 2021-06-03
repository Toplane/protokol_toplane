using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace ProtokolApp
{
    public partial class KreiranjeUsera : Telerik.WinControls.UI.RadForm
    {
        public KreiranjeUsera()
        {
            InitializeComponent();
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            RadSaveFileDialog fd = new RadSaveFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                if (EntityConnectionString.SacuvajEntityConnectionString(txtUser.Text, txtPassword.Text, txtServer.Text,
                    txtBaza.Text, fd.FileName))
                {
                    RadMessageBox.ThemeName = "Office2010Silver";
                    RadMessageBox.Show("Uspjesno sacuvano");
                }

            }

            
        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            EntityConnectionString connectionString = new EntityConnectionString();
            RadOpenFileDialog fd = new RadOpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {

                txtConString.Text = Helper.ProcitajEntityConnectionString();
            }
            
        }
    }
}
