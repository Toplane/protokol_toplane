using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ProtokolApp
{
    public partial class UnosSP : Telerik.WinControls.UI.RadForm
    {
        private Form childForm;
        private int idKorisnika;
        string korisniKIme = Environment.UserName;

        public UnosSP(Form forma)
        {
            InitializeComponent();
            childForm = forma;

            radDateTimePicker1.Value = DateTime.Now;
            radDateTimePicker2.Value = DateTime.Now;

            

            idKorisnika = Helper.GetIDKorisnika(korisniKIme);


            List<sluzbe> sluz = Helper.GetSluzbeZaKorisnika(idKorisnika);

            this.numericUpDown1.Value = Helper.GetNextMaxBrojProtokola(6, DateTime.Now.Year);
        }

        private void txtIznosPoRjesenju_TextChanged(object sender, EventArgs e)
        {
            decimal iznosPoRjesenju;
            if (!decimal.TryParse(txtIznosPoRjesenju.Text, out iznosPoRjesenju))
            {
                txtIznosPoRjesenju.Clear();
                RadMessageBox.SetThemeName("Office2010Silver");
                RadMessageBox.Show("U polje iznos ide iskljucivo decimalni broj!");
                return;
                
            }
        }

        private void UnosSP_FormClosing(object sender, FormClosingEventArgs e)
        {
            childForm.Enabled = true;
            childForm.Show();
        }
    }
}
