using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.DirectoryServices;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using System.IO;

namespace ProtokolApp
{
    public partial class frmOsnovna : Form
    {
        private static int _idSluzbe = 0;
        private static int _idKorisnika = 0;



        public frmOsnovna()
        {
            InitializeComponent();
        }


        private protokolEntities p;
        private async void FrmOsnovni_Load(object sender, EventArgs e)
        {

            _idSluzbe = Helper.GetIDSluzbe(Environment.UserName);
            _idKorisnika = Helper.GetIDKorisnika(Environment.UserName);

            string user = System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName;
            this.Text = "Protokol - " + user + " - "+ Helper.GetNazivSluzbe(_idKorisnika);
            radGridView1.DataSource = await Task.Run(() => Citaj());

        }

        private async Task<BindingList<protokol>> Citaj()
        {
            p = new protokolEntities();
            await p.protokol.Where(pr => pr.ID_sluzbe == _idSluzbe && pr.izbrisan == 0).LoadAsync();
            return p.protokol.Local.ToBindingList();
        }




        private void radButton2_Click(object sender, EventArgs e)
        {
            try
            {
                p.SaveChanges();
                MessageBox.Show("Izmjene spašene");
            }

            catch
            {
                MessageBox.Show("Došlo je do greške!");
            }

        }


        private void radGridView1_DataBindingComplete_1(object sender, GridViewBindingCompleteEventArgs e)
        {

            if (radGridView1.Columns["ime"] == null)
            {
                p = new protokolEntities();
                var tipovi = p.tip.Select(pr => pr).ToList();
                GridViewComboBoxColumn tipDokumentaColumn = new GridViewComboBoxColumn();
                tipDokumentaColumn.Name = "ime";
                tipDokumentaColumn.HeaderText = "Tip dokumenta";
                tipDokumentaColumn.DataSource = tipovi;
                tipDokumentaColumn.ValueMember = "ID";
                tipDokumentaColumn.DisplayMember = "Naziv";
                tipDokumentaColumn.FieldName = "dddd";
                tipDokumentaColumn.Width = 200;
                tipDokumentaColumn.DropDownStyle = RadDropDownStyle.DropDownList;
                
                this.radGridView1.Columns.Add(tipDokumentaColumn);
            }
            for (int i = 0; i < radGridView1.RowCount; i++)
            {
                radGridView1.Rows[i].Cells["ime"].Value = radGridView1.Rows[i].Cells["ID_tipa"].Value;
            }

        }

        private void radGridView1_CellValueChanged(object sender, GridViewCellEventArgs e)
        {
            radGridView1.CurrentRow.Cells["ID_tipa"].Value = radGridView1.CurrentRow.Cells["ime"].Value;
            
        }


        private void radButton1_Click_1(object sender, EventArgs e)
        {
            Unos unos = new Unos(this);
            this.Enabled = false;
            unos.Show();

        }

        void radGridView1_Izbrisi(object sender, GridViewCellEventArgs e)
        {
            
            if (e.Column.Name == "Izbrisi")
            {
                if (RadMessageBox.Show("Da li ste sigurni da želite izbrisati file iz baze?", "Pitanje", MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
                    int id = 0;
                    int.TryParse(radGridView2.Rows[e.RowIndex].Cells["id"].Value.ToString(), out id);

                    p = new protokolEntities();

                    dokument doc = new dokument();

                    doc = p.dokument.Where(d => d.ID == id).FirstOrDefault();
                    doc.Izbrisan = 1;
                    p.SaveChanges();
                    e.Row.Delete();


                }

            }

        }

        void radGridView1_CommandCellClick(object sender, GridViewCellEventArgs e)
        {
            if (e.Column.Name == "Command")
            {
                int id = 0;
                int.TryParse(radGridView2.Rows[e.RowIndex].Cells["id"].Value.ToString(), out id);

                string putanjaDirektorija = Directory.GetCurrentDirectory() + "\\files\\";
                string imeFilea = radGridView2.Rows[e.RowIndex].Cells["Filename"].Value.ToString();


                string filename = putanjaDirektorija +
                                  imeFilea;
                try
                {
                    if (!Directory.Exists(putanjaDirektorija))
                    {
                        Directory.CreateDirectory(putanjaDirektorija);
                    }
                }
                catch
                {

                }

                databaseFileRead(id, filename);
                try
                {
                    Process.Start(filename);
                }
                catch
                {
                    MessageBox.Show("doslo je do greske");
                }
            }

        }

        private void radGridView2_RowFormatting(object sender, RowFormattingEventArgs e)
        {

        }

        public static bool databaseFileRead(int id, string putanja)
        {
            try
            {
                protokolEntities p = new protokolEntities();

                var binarniPodaci = p.dokument.Where(d => d.ID == id).Select(d => d.Dokument).FirstOrDefault();

                using (var fs = new FileStream(putanja, FileMode.Create, FileAccess.Write))
                    fs.Write(binarniPodaci, 0, binarniPodaci.Length);
                return true;
            }
            catch
            {
                return false;
            }


        }

        private void radGridView1_CurrentRowChanged(object sender, CurrentRowChangedEventArgs e)
        {

            if (radGridView1.CurrentRow.Cells["ID"].Value != null)
            {
                int idProtokola = (int)radGridView1.CurrentRow.Cells["ID"].Value;

                p = new protokolEntities();

                p.dokument.Where(d => d.ID_protokola == idProtokola && d.Izbrisan == 0).Load();

                radGridView2.DataSource = p.dokument.Local.ToBindingList();
            }

        }

        private void radGridView2_DataBindingComplete(object sender, GridViewBindingCompleteEventArgs e)
        {
            if (radGridView2.Columns["Command"] == null)
            {
                // Add command column (containing button) 
                GridViewCommandColumn commandColumn = new GridViewCommandColumn();
                commandColumn.Name = "Command";
                commandColumn.HeaderText = "";
                commandColumn.Width = 100;
                commandColumn.DefaultText = "Otvori file";
                commandColumn.UseDefaultText = true;
                
                radGridView2.Columns.Add(commandColumn);
                radGridView2.Columns.Move(7, 0);

                radGridView2.CommandCellClick += new CommandCellClickEventHandler(radGridView1_CommandCellClick);
            }
            if (radGridView2.Columns["Tipfilea"] == null)
            {
                // Add command column (containing button) 
                GridViewTextBoxColumn commandColumn = new GridViewTextBoxColumn();
                commandColumn.Name = "Tipfilea";
                commandColumn.HeaderText = "Tip filea";
                commandColumn.Width = 300;
                commandColumn.AutoSizeMode = BestFitColumnMode.SummaryRowCells;
                radGridView2.Columns.Add(commandColumn);
            }

            if (radGridView2.Columns["Izbrisi"] == null)
            {
                // Add command column (containing button) 
                GridViewCommandColumn commandColumn = new GridViewCommandColumn();
                commandColumn.Name = "Izbrisi";
                commandColumn.HeaderText = "Izbrisi";
                commandColumn.Width = 160;
                commandColumn.DefaultText = "Izbrisi file";
                commandColumn.UseDefaultText = true;
                commandColumn.AutoSizeMode = BestFitColumnMode.SummaryRowCells;
                radGridView2.Columns.Add(commandColumn);
                

                radGridView2.CommandCellClick += new CommandCellClickEventHandler(radGridView1_Izbrisi);
            }
            for (int i = 0; i < radGridView2.RowCount; i++)
            {
                if ((int)radGridView2.Rows[i].Cells["TipDokumenta"].Value==1)
                {
                    radGridView2.Rows[i].Cells["Tipfilea"].Value = "Predmet";
                }
                if ((int)radGridView2.Rows[i].Cells["TipDokumenta"].Value == 2)
                {
                    radGridView2.Rows[i].Cells["Tipfilea"].Value = "Veza";
                }
            }


            this.radGridView2.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;

            this.radGridView2.Columns[0].BestFit();
            this.radGridView2.Columns[1].BestFit();
            this.radGridView2.Columns[2].BestFit();
            this.radGridView2.Columns[3].BestFit();
        }

        private async void frmOsnovna_EnabledChanged(object sender, EventArgs e)
        {
            if (this.Enabled)
            {
                radGridView1.DataSource = await Task.Run(() => Citaj());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
