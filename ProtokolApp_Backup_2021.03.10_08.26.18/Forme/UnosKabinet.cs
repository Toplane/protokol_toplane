using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;


namespace ProtokolApp
{
    public partial class UnosKabinet : Telerik.WinControls.UI.RadForm
    {
        private Form f1;

        static int idKorisnika = 0;
        static int idSluzbe = 0;

        public UnosKabinet(Form form1, int idSluzbeSelected)
        {
            f1 = form1;
            //if (idSluzbeSelected == 6)
            //{
            //    this.WindowState = FormWindowState.Maximized;
            //    radGroupBox2.Visible = true;
            //}
            //else
            //{

            //}


            

            InitializeComponent();
            ddUlazIzlaz.Items.Add(new RadListDataItem("Ulaz", 1));
            ddUlazIzlaz.Items.Add(new RadListDataItem("Izlaz", 2));
            ddUlazIzlaz.Items.Add(new RadListDataItem("Ulaz i izlaz", 3));
            ddUlazIzlaz.SelectedIndex = 0;
            ddUlazIzlaz.DropDownStyle= Telerik.WinControls.RadDropDownStyle.DropDownList;

            AcceptButton = rbnUnos;
          //  this.FormBorderStyle = FormBorderStyle.FixedDialog;

            string korisniKIme = Environment.UserName;

            idKorisnika = Helper.GetIDKorisnika(korisniKIme);



                List<sluzbe> sluz = Helper.GetSluzbeZaKorisnika(idKorisnika);
            if (sluz.Count > 0)
            {
                radDropDownList1.DataSource = sluz;
                radLabel2.Visible = true;
                radDropDownList1.Visible = true;
                idSluzbe = (int) radDropDownList1.SelectedItem.Value;
            }

            else
            {
                MessageBox.Show("Korisnik racunara nije definiran u bazi, kontaktirajte administratora");
                //  idSluzbe = Helper.GetIDSluzbe(korisniKIme);
            }

            try
            {
                radDropDownList1.SelectedValue = idSluzbeSelected;
            }
            catch (Exception e)
            {
                MessageBox.Show("Korisnik racunara nije definiran u bazi, kontaktirajte administratora");
            }

            numericUpDown1.Value = Helper.GetNextMaxBrojProtokola(idSluzbe, DateTime.Now.Year);
            //txtRedniBroj.Text = Helper.GetNextMaxBrojProtokola(idSluzbe, DateTime.Now.Year).ToString();


            Text = "Unos - " + korisniKIme + " - " + radDropDownList1.SelectedItem.Text +  " protokol";

            dtDatum.Value = DateTime.Now;

        }

        private void Unos_FormClosing(object sender, FormClosingEventArgs e)
        {
            f1.Enabled = true;
            f1.Show();
        }

        private void rbOdaberiFile_Click(object sender, EventArgs e)
        {
            if (radOpenFilePredmet.ShowDialog()==DialogResult.OK)
            {
                rbOdaberiFile.Text = "File predmeta odabran";
                lblFilePathPredmet.Text = radOpenFilePredmet.FileName;
                rbnUkloniPredmet.Visible = true;
            }
        }

        private void rbnUkloniPredmet_Click(object sender, EventArgs e)
        {
            lblFilePathPredmet.Text = "File nije odabran";
            rbOdaberiFile.Text = "File nije odabran";
            rbnUkloniPredmet.Visible = false;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            if (radOpenFileVeza.ShowDialog()==DialogResult.OK)
            {
                rbnOdaberiFileVeza.Text = "File predmeta odabran";
                lblFilePathVeza.Text = radOpenFileVeza.FileName;
                rbnUkloniFileVeza.Visible = true;
            }
        }

        private void rbnUkloniFileVeza_Click(object sender, EventArgs e)
        {
            lblFilePathVeza.Text = "File nije odabran";
            rbnOdaberiFileVeza.Text = "File nije odabran";
            rbnUkloniFileVeza.Visible = false;
        }

        private void rbnUnos_Click(object sender, EventArgs e)
        {

            int rednibroj;
            int.TryParse(numericUpDown1.Value.ToString(), out rednibroj);
            int tip = (int)ddUlazIzlaz.SelectedValue;
            DateTime datu = dtDatum.Value;
            protokolKabinetDTO protokolKabinet = new protokolKabinetDTO();
            protokolKabinet.ID_sluzbe = idSluzbe;
            protokolKabinet.datum = dtDatum.Value;
            protokolKabinet.napomena = txtNapomena.Text;
            protokolKabinet.ID_tipa = (int)ddUlazIzlaz.SelectedItem.Value;
            protokolKabinet.podbroj_Kabinet = txtPodbroj.Text;
            protokolKabinet.posiljalac_Kabinet = txtPosiljalacKabinet.Text;
            protokolKabinet.predmet = txtNazivPredmeta.Text;
            protokolKabinet.razvod = txtRazvod.Text;
            protokolKabinet.redni_broj =(int) numericUpDown1.Value;
            protokolKabinet.redniBroj_Kabinet = txtRedniBrojKabinet.Text;
            protokolKabinet.napomena = txtNapomena.Text;

            //bool uspjeh = Helper.UnesiProtokolKabinet(idSluzbe, rednibroj, tip, datu, txtNazivPredmeta.Text, txtNazivVeze.Text, lblFilePathPredmet.Text,
            //    lblFilePathVeza.Text, txtNapomena.Text, txtRazvod.Text);
            protokolKabinetDTO.Save(protokolKabinet);
            RadMessageBox.ThemeName= "Office2010Silver";
          //  RadMessageBox.Show(uspjeh ? "Uspjesno uneseno" : "Doslo je do greske");
            Close();


        }

        private void radDropDownList1_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            idSluzbe = (int) radDropDownList1.SelectedItem.Value;
            numericUpDown1.Value = Helper.GetNextMaxBrojProtokola(idSluzbe, dtDatum.Value.Year);
            this.Text = "Unos - " + Environment.UserName + " - " + radDropDownList1.SelectedItem.Text + " protokol";
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                var protokolListDataAutocomplete = context.protokol.Where(p => p.veza.Length > 0 && p.ID_sluzbe == idSluzbe).Select(p => p.veza).Distinct().ToList();
                protokolListDataAutocomplete.Insert(0, null);
                txtNazivVeze.AutoCompleteDataSource = protokolListDataAutocomplete;
                txtNazivVeze.DataSource = protokolListDataAutocomplete;
                txtNazivVeze.AutoCompleteMode = AutoCompleteMode.SuggestAppend;


                var rednibrojKabinetAutoComplete = context.protokol
                    .Where(p => p.redniBroj_Kabinet.Length > 0 && p.ID_sluzbe == idSluzbe).OrderByDescending(p => p.datum)
                    .Select(p => p.redniBroj_Kabinet).ToList();
                rednibrojKabinetAutoComplete.Insert(0, null);
                txtRedniBrojKabinet.AutoCompleteDataSource = rednibrojKabinetAutoComplete;
                txtRedniBrojKabinet.DataSource = rednibrojKabinetAutoComplete;
                txtRedniBrojKabinet.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                var oznakaregistratoraAutoCompleteList =
                    context.protokol.Where(or=>or.oznaka_registratora.Length>0 && or.ID_sluzbe==idSluzbe).Select(or => or.oznaka_registratora).Distinct().ToList();
                //txtOznakaRegistratora.AutoCompleteDataSource = oznakaregistratoraAutoCompleteList;
                //txtOznakaRegistratora.AutoCompleteMode = AutoCompleteMode.SuggestAppend;


            }
        }

        private void dtDatum_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = Helper.GetNextMaxBrojProtokola(idSluzbe, dtDatum.Value.Year);
        }
    }
}
