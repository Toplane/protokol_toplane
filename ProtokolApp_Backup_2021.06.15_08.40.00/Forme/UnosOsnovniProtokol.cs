using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Forms;
using ProtokolApp.Klase;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using Telerik.WinControls;
using Telerik.WinControls.UI;


namespace ProtokolApp
{

    public partial class UnosOsnovniProtokol : Telerik.WinControls.UI.RadForm
    {
        private FormC f1;

        static int idKorisnika = 0;
        static int idSluzbe = 0;
        private protokol p;
        private SqlTableDependency<protokol> depProtokol;
        public UnosOsnovniProtokol(FormC form1, int idSluzbeSelected, protokol Protokol)
        {
            f1 = form1;
            p = Protokol;
            //if (idSluzbeSelected == 6)
            //{
            //    this.WindowState = FormWindowState.Maximized;
            //    radGroupBox2.Visible = true;
            //}
            //else
            //{

            //}

            depProtokol = new SqlTableDependency<protokol>(Helper.ProcitajConnectionString(), null, null, null, null, null, DmlTriggerType.All, true, true);
            depProtokol.OnChanged += IzmjeneProtokolEventHandler;
            depProtokol.Start();


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


        private void IzmjeneProtokolEventHandler(object o, RecordChangedEventArgs<protokol> e)
        {
            if (e.ChangeType == ChangeType.Insert || e.ChangeType == ChangeType.Update)
            {
                numericUpDown1.Invoke((MethodInvoker) delegate
                {
                    numericUpDown1.Value = Helper.GetNextMaxBrojProtokola(idSluzbe, DateTime.Now.Year);
                });

            }

        }
        private void Unos_FormClosing(object sender, FormClosingEventArgs e)
        {
            f1.Show(p);
            f1.Enabled = true;
           
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
            int rednibroj2= Helper.GetNextMaxBrojProtokola(idSluzbe, dtDatum.Value.Year);
            if (rednibroj2 != rednibroj)
            {
                DialogResult d= RadMessageBox.Show(
                                     string.Format(
                                         "Unesen je novi redni broj {0}, stari broj je bio {1}, zelite li sacuvati unos pod novim rednim brojem?",
                                         rednibroj2, rednibroj), "Pitanje", MessageBoxButtons.YesNoCancel);
                if (d==DialogResult.Yes)
                {
                    rednibroj = rednibroj2;
                }

                if (d == DialogResult.Cancel)
                {
                    Close();
                    return;
                }
                
            }
            bool uspjeh=Helper.UnosProtokolUBazu(idSluzbe, rednibroj, tip, datu, txtNazivPredmeta.Text, txtNazivVeze.Text, lblFilePathPredmet.Text,
                lblFilePathVeza.Text, txtOznakaRegistratora.Text, txtArhiva.Text, txtOznakaDopisa.Text,
                txtDostavaDopisa.Text, txtNapomena.Text, txtRazvod.Text);
            RadMessageBox.ThemeName= "Office2010Silver";
            RadMessageBox.Show(uspjeh!=false  ? "Uspjesno uneseno" : "Doslo je do greske");


            //p = Helper.GetSingle(uspjeh.ID); 
            //Ovo radimo zato sto nije dovoljno novi objekat koji vrati UnosProtokolUBazu vratiti u formu. 
            //Razlog tome je kalkulirano polje, cntBrojDokumenata u bazi, koje je u entity frameworku readonly i kalkulira se u bazi.
            //Dakle, spasimo u bazu, pa ga uzmemo iz baze.
            Close();


        }

        private void radDropDownList1_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            idSluzbe = (int) radDropDownList1.SelectedItem.Value;
            PromijeniLabele((int)radDropDownList1.SelectedItem.Value);
            numericUpDown1.Value = Helper.GetNextMaxBrojProtokola(idSluzbe, dtDatum.Value.Year);
            this.Text = "Unos - " + Environment.UserName + " - " + radDropDownList1.SelectedItem.Text + " protokol";
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                var protokolListDataAutocomplete = context.protokol.Where(p => p.veza.Length > 0 && p.ID_sluzbe == idSluzbe).Select(p => p.veza).Distinct().ToList();
                protokolListDataAutocomplete.Insert(0, null);
                txtNazivVeze.AutoCompleteDataSource = protokolListDataAutocomplete;
                txtNazivVeze.DataSource = protokolListDataAutocomplete;
                txtNazivVeze.AutoCompleteMode = AutoCompleteMode.SuggestAppend;


                var razvodAutocompleteAutocompleteList = context.protokol.Where(r => r.razvod.Length > 0 && r.ID_sluzbe == idSluzbe)
                    .Select(r => r.razvod).Distinct().ToList();
                razvodAutocompleteAutocompleteList.Insert(0, null);
                txtRazvod.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtRazvod.DataSource = razvodAutocompleteAutocompleteList;
                txtRazvod.AutoCompleteDataSource = razvodAutocompleteAutocompleteList;

                var oznakaregistratoraAutoCompleteList =
                    context.protokol.Where(or=>or.oznaka_registratora.Length>0 && or.ID_sluzbe==idSluzbe).Select(or => or.oznaka_registratora).Distinct().ToList();
                txtOznakaRegistratora.AutoCompleteDataSource = oznakaregistratoraAutoCompleteList;
                txtOznakaRegistratora.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                var arhivaAutocompleteList = context.protokol.Where(p => p.arhiva.Length > 0 && p.ID_sluzbe == idSluzbe)
                    .Select(p => p.arhiva).Distinct().ToList();
                arhivaAutocompleteList.Insert(0, null);
                txtArhiva.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtArhiva.AutoCompleteDataSource = arhivaAutocompleteList;
                txtArhiva.DataSource = arhivaAutocompleteList;


            }

        }

        private void dtDatum_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = Helper.GetNextMaxBrojProtokola(idSluzbe, dtDatum.Value.Year);
        }

        private void UnosOsnovniProtokol_Load(object sender, EventArgs e)
        {
           PromijeniLabele((int)radDropDownList1.SelectedItem.Value);
        }

        private void PromijeniLabele(int idSluzbe)
        {
            switch (idSluzbe)
            {
                case 9:
                    lblNaziv.Text = "Broj računa";
                    lblVeza.Text = "Naziv Službe (SN, SOP...)";
                    lblRazvod.Text = "Pošiljalac";
                    lblDostavaDopisa.Text = "Iznos";

                    txtNazivPredmeta.NullText = "Broj računa";
                    txtNazivVeze.NullText= "Naziv Službe (SN, SOP...)";
                    txtRazvod.NullText= "Pošiljalac";
                    txtDostavaDopisa.NullText = "Iznos";
                    break;
                case 1:
                    lblNaziv.Text = "Naziv predmeta";
                    lblVeza.Text = "Naziv veze";
                    lblRazvod.Text = "Razvod";
                    lblDostavaDopisa.Text = "Dostava dopisa";

                    txtNazivPredmeta.NullText = "Naziv predmeta";
                    txtNazivVeze.NullText = "Naziv veze";
                    txtRazvod.NullText = "Razvod";
                    txtDostavaDopisa.NullText = "Dostava dopisa";
                    break;
            }
        }
    }
}
