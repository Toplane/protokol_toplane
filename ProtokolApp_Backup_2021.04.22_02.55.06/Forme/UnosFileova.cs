using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace ProtokolApp
{
    public partial class UnosFileova : Telerik.WinControls.UI.RadForm
    {
        RadForm form;
        int idprotokola;
       
        public UnosFileova(int idProtokola, RadForm forma, string opisPredmet, string opisVeza)
        {
            form = forma;
            
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            radTextBoxPredmet.Text = opisPredmet;
            radTextBoxVeza.Text = opisVeza;
            idprotokola = idProtokola;
        }


        private void UnosFileova_FormClosing(object sender, FormClosingEventArgs e)
        {
            form.Enabled = true;
            form.Show();
        }

        private void radButtonFilePredmeta_Click(object sender, EventArgs e)
        {
            if (radOpenFilePredmet.ShowDialog()==DialogResult.OK)
            {
                radLabelPredmet.Text = radOpenFilePredmet.FileName;
                radButtonFilePredmeta.Text = "File odabran";
                radButtonUkloniPredmet.Visible = true;
            }
        }

        private void radButtonUkloniPredmet_Click(object sender, EventArgs e)
        {
            radLabelPredmet.Text = "";
            radButtonFilePredmeta.Text = "Odaberite file predmeta";
            radButtonUkloniPredmet.Visible = false;
        }

        private void radButtonFileVeze_Click(object sender, EventArgs e)
        {
            if (radOpenFileVeza.ShowDialog() == DialogResult.OK)
            {
                radLabelVeza.Text = radOpenFileVeza.FileName;
                radButtonFileVeze.Text = "File odabran";
                radButtonUkloniFileVeze.Visible = true;
            }
        }

        private void radButtonUnos_Click(object sender, EventArgs e)
        {
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                if (idprotokola>0)
                {
                    var p = context.protokol.Where(d => d.ID == idprotokola).FirstOrDefault();

                    if (File.Exists(radLabelPredmet.Text))
                    {
                        if (radLabelPredmet.Text.EndsWith(".pdf"))
                        {
                            dokument d = new dokument { TipDokumenta = 1, Dokument = File.ReadAllBytes(radLabelPredmet.Text) };
                            d.Filename = radLabelPredmet.Text.Substring(radLabelPredmet.Text.LastIndexOf('\\') + 1);
                            d.Opis = radTextBoxPredmet.Text;
                            d.Izbrisan = 0;
                            d.Racunar = Environment.MachineName;
                            p.dokument.Add(d);
                        }
                    }


                    if (File.Exists(radLabelVeza.Text))
                    {
                        if (radLabelVeza.Text.EndsWith(".pdf"))
                        {
                            dokument d = new dokument { TipDokumenta = 2, Dokument = File.ReadAllBytes(radLabelVeza.Text) };
                            d.Filename = radLabelVeza.Text.Substring(radLabelVeza.Text.LastIndexOf('\\') + 1);
                            d.Opis = radTextBoxVeza.Text;
                            d.Izbrisan = 0;
                            d.Racunar = Environment.MachineName;
                            p.dokument.Add(d);
                        }
                    }

                }
                try
                {
                    context.SaveChanges();
                    Close();
                }
                catch
                {
                    RadMessageBox.Show("doslo je do greske!!");
                }
                
            }
        }

        private void radButtonUkloniFileVeze_Click(object sender, EventArgs e)
        {
            radLabelVeza.Text = "";
            radButtonFileVeze.Text = "Odaberite file predmeta";
            radButtonUkloniFileVeze.Visible = false;
        }
    }
}
