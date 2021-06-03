﻿using System;
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
using System.Threading;
using System.Xml;
using Telerik.WinControls.UI.Localization;
using Telerik.WinControls.Export;
using Telerik.WinControls.UI.Export;
using ClosedXML.Excel;

namespace ProtokolApp
{
    public partial class RadFormOsnovniProtokol : Telerik.WinControls.UI.RadForm
    {
        private static int _idSluzbe = 0;
        private static int _idKorisnika = 0;
        private bool editirano = false;
        

        public class MYCustomLocalizationProvider : RadGridLocalizationProvider
        {
            public override string GetLocalizedString(string id)
            {
                switch (id)
                {
                    case RadGridStringId.GroupByThisColumnMenuItem:
                        return "Grupacija";
                    case RadGridStringId.GroupingPanelHeader:
                        return "Grupirano prema";
                    case RadGridStringId.GroupingPanelDefaultMessage:
                        return "Prevucite kolonu na ovo mjesto da grupisete po njoj";
                    case RadGridStringId.CustomFilterMenuItem:
                        return "Filtriraj prema";
                    case RadGridStringId.SearchRowMatchCase:
                        return "Uzmi u obzir VELIKA i mala slova";
                    case RadGridStringId.FilterFunctionContains:
                        return "Sadrzi";
                    case RadGridStringId.SearchRowTextBoxNullText:
                        return "Unesite pojam za pretragu";
                        
                    

                }

                return base.GetLocalizedString(id);
            }
        }

        public RadFormOsnovniProtokol()
        {

            
            InitializeComponent();
        }


        private protokolEntities1 mainProtokolEntities;
        private protokolEntities1 dokumentProtokolEntities;
        private  void FrmOsnovni_Load(object sender, EventArgs e)
        {
            
            try
            {
                _idKorisnika = Helper.GetIDKorisnika(Environment.UserName);
                
            }
            catch
            {
                MessageBox.Show("Greska u komunikaciji sa bazom");
            }

            if (_idKorisnika == 0)
            {
                MessageBox.Show("Korisnik nije definiran u bazi");
            }
            
            

            List < sluzbe > sluzbeList = Helper.GetSluzbeZaKorisnika(_idKorisnika);
            if (sluzbeList.Count > 0)
            {
                radDropDownList1.Visible = true;
                radDropDownList1.DataSource = sluzbeList;
                radLabel1.Visible = true;
                radDropDownList1.Visible = true;

            }

            
            else
            {
                //_idSluzbe = Helper.GetIDSluzbe(Environment.UserName);
            }

            if (!Helper.CanEdit(Environment.UserName))
            {
                radGridView1.AllowEditRow = false;
                radButton3.Enabled = false;
                radButton4.Enabled = false;
            }

            if (!Helper.CanNew(Environment.UserName))
            {
                radButton2.Enabled = false;
            }

            if (!Helper.CanInsertNewFile(Environment.UserName))
            {
                radButton4.Enabled = false;
            }

            string user = System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName;
            this.Text = "Protokol - " + user + " - " + radDropDownList1.SelectedItem.Text;
            if (File.Exists(Directory.GetCurrentDirectory() + "\\MainLayout.xml"))
            {
                radGridView1.LoadLayout(Directory.GetCurrentDirectory() + "\\MainLayout.xml");
            }
            //radGridView2.Rows.Clear();
            //radGridView1.DataSource = await Task.Run(() => Citaj(_idSluzbe));

            RadGridLocalizationProvider.CurrentProvider = new MYCustomLocalizationProvider();


            radGridView1.EditorRequired += radGridView1_EditorRequired;
            radGridView1.CellEditorInitialized += radGridView1_CellEditorInitialized;


        }




        public async Task<BindingList<protokol>> Citaj(int id_sluzbe)
        {
            try
            {

                mainProtokolEntities = new protokolEntities1(Helper.ProcitajEntityConnectionString());
                if (id_sluzbe != 6)
                {
                    //await mainProtokolEntities.protokol.Where(pr => pr.ID_sluzbe == id_sluzbe && pr.izbrisan == 0)
                    //    .OrderByDescending(ii => ii.ID).LoadAsync();
                    await mainProtokolEntities.protokol.Include(s => s.dokument).Where(pr => pr.ID_sluzbe == id_sluzbe && pr.izbrisan == 0)
                        .OrderByDescending(ii => ii.ID).LoadAsync();
                }
                else
                {
                    await mainProtokolEntities.protokol.Include(s=>s.dokument).Where(pr => pr.ID_sluzbe == id_sluzbe && pr.izbrisan == 0)
                        .OrderByDescending(ii => ii.ID).LoadAsync();

                }

                return mainProtokolEntities.protokol.Local.ToBindingList();
                
            }
            catch(Exception ex)
            {
                return null;
            }

        }

        private BindingList<dokument> CitajDokumentList(int idProtokola)
        {
            dokumentProtokolEntities = new protokolEntities1(Helper.ProcitajEntityConnectionString());
            dokumentProtokolEntities.dokument.Where(pr => pr.ID_protokola == idProtokola && pr.Izbrisan == 0);
          
            BindingList<dokument> dokList = new BindingList<dokument>(dokumentProtokolEntities.dokument.Where(pr => pr.ID_protokola == idProtokola && pr.Izbrisan == 0).ToList());
            return dokList;
        }





        private void radGridView1_DataBindingComplete_1(object sender, GridViewBindingCompleteEventArgs e)
        {
            
            if (radGridView1.Columns["ime"] == null)
            {
                protokolEntities1 pro = new protokolEntities1(Helper.ProcitajEntityConnectionString());
                var tipovi = pro.tip.Select(pr => pr).ToList();
                GridViewComboBoxColumn tipDokumentaColumn = new GridViewComboBoxColumn();
               
                tipDokumentaColumn.Name = "ime";
                tipDokumentaColumn.HeaderText = "Tip dokumenta";
                tipDokumentaColumn.DataSource = tipovi;
                tipDokumentaColumn.ValueMember = "ID";
                tipDokumentaColumn.DisplayMember = "Naziv";
                tipDokumentaColumn.FieldName = "dddd";
                tipDokumentaColumn.Width = 150;
                tipDokumentaColumn.DropDownStyle = RadDropDownStyle.DropDownList;

                this.radGridView1.Columns.Add(tipDokumentaColumn);
            }

            if (Helper.CanEdit(Environment.UserName))
            {
                if (radGridView1.Columns["Izbrisi"] == null)
                {
                    // Add command column (containing button) 
                    GridViewCommandColumn commandColumn = new GridViewCommandColumn();
                    commandColumn.Name = "Izbrisi";
                    commandColumn.HeaderText = "";
                    commandColumn.Width = 160;
                    commandColumn.DefaultText = "Izbrisi unos";
                    commandColumn.UseDefaultText = true;
                    commandColumn.AutoSizeMode = BestFitColumnMode.SummaryRowCells;
                    commandColumn.AllowSort = false;

                    radGridView1.Columns.Add(commandColumn);


                    radGridView1.CommandCellClick += new CommandCellClickEventHandler(radGridView_protokol_Izbrisi);
                }
            }

            //if (radGridView1.Columns["Hy"] == null)
            //{
            //    GridViewHyperlinkColumn hyperlinkColumn = new GridViewHyperlinkColumn("Hy");
            ////    hyperlinkColumn.ExcelExportType = DisplayFormatType.None;
            //    radGridView1.Columns.Add(hyperlinkColumn);
            //}



            for (int i = 0; i < radGridView1.RowCount; i++)
            {
                radGridView1.Rows[i].Cells["ime"].Value = radGridView1.Rows[i].Cells["ID_tipa"].Value;
            }

            radGridView1.MasterView.TableSearchRow.ResumeSearch();

        }

        private void radGridView1_CellValueChanged(object sender, GridViewCellEventArgs e)
        {
            if (radGridView1.CurrentRow.Cells["ime"].Value != null)
            {
                radGridView1.CurrentRow.Cells["ID_tipa"].Value = radGridView1.CurrentRow.Cells["ime"].Value;
            }

            
            
        }


        private void radButton1_Click_1(object sender, EventArgs e)
        {
            RadMessageBox.ThemeName = "Office2010Silver";
            if (Helper.CanEdit(Environment.UserName)&&editirano)
            {
                if (RadMessageBox.Show("Da li zelite spasiti izmjene?", "Pitanje", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    radButton3.PerformClick();
                }
            }
            radGridView1.MasterView.TableSearchRow.SuspendSearch();
            UnosOsnovniProtokol unos = new UnosOsnovniProtokol(this, (int)radDropDownList1.SelectedItem.Value);
            this.Enabled = false;
            
            unos.Show();

        }

        void radGridView1_Izbrisi(object sender, GridViewCellEventArgs e)
        {
            if (Helper.CanEdit(Environment.UserName))
            {
                if (e.Column.Name == "Izbrisi")
                {
                    RadMessageBox.SetThemeName("Office2010Silver");
                    if (RadMessageBox.Show("Da li ste sigurni da želite izbrisati file iz baze?", "Pitanje", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        int id = 0;
                        int.TryParse(radGridView2.Rows[e.RowIndex].Cells["id"].Value.ToString(), out id);

                        protokolEntities1 docu = new protokolEntities1(Helper.ProcitajEntityConnectionString());

                        dokument doc = new dokument();

                        doc = docu.dokument.Where(d => d.ID == id).FirstOrDefault();
                        doc.Izbrisan = 1;
                        docu.SaveChanges();
                        e.Row.Delete();

                    }

                }
            }

        }

        void radGridView_protokol_Izbrisi(object sender, GridViewCellEventArgs e)
        {
            if (Helper.CanEdit(Environment.UserName))
            {
                if (e.Column.Name == "Izbrisi")
                {
                    RadMessageBox.SetThemeName("Office2010Silver");
                    if (RadMessageBox.Show("Da li ste sigurni da želite izbrisati unos?", "Pitanje", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        int id = 0;
                        int.TryParse(radGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString(), out id);

                        protokolEntities1 docu = new protokolEntities1(Helper.ProcitajEntityConnectionString());

                        protokol _pr = new protokol();

                        _pr = docu.protokol.Where(d => d.ID == id).FirstOrDefault();
                        _pr.izbrisan = 1;
                        docu.SaveChanges();
                        e.Row.Delete();
                        try
                        {
                            foreach (GridViewRowInfo r in radGridView2.Rows)
                            {
                                if (r.Cells["ID_protokola"].Value.ToString() == id.ToString())
                                {
                                    r.Delete();
                                }
                            }
                        }
                        catch
                        {

                        }


                    }

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


        public static bool databaseFileRead(int id, string putanja)
        {
            try
            {
                using (protokolEntities1 pro = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
                {

                    var binarniPodaci = pro.dokument.Where(d => d.ID == id && d.Izbrisan==0).Select(d => d.Dokument).FirstOrDefault();

                    using (var fs = new FileStream(putanja, FileMode.Create, FileAccess.Write))
                        fs.Write(binarniPodaci, 0, binarniPodaci.Length);
                    return true;
                }

            }
            catch
            {
                return false;
            }


        }

        private void radGridView1_CurrentRowChanged(object sender, CurrentRowChangedEventArgs e)
        {
            try
            {
                if (radGridView1.CurrentRow.Cells["ID"].Value != null)
                {
                    int idProtokola = (int)radGridView1.CurrentRow.Cells["ID"].Value;
                    radGridView2.DataSource = CitajDokumentList(idProtokola);
                    string predmet = radGridView1.CurrentRow.Cells["predmet"].Value != null ? radGridView1.CurrentRow.Cells["predmet"].Value.ToString() : "";
                    radGroupBox3.Text = "Dokumenti: " + "redni broj: " + radGridView1.CurrentRow.Cells["redni_broj"].Value.ToString() + ", predmet: " + predmet;
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.ToString());
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
                radGridView2.Columns.Move(8, 0);

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

            if (Helper.CanEdit(Environment.UserName))
            {
                if (radGridView2.Columns["Izbrisi"] == null)
                {
                    // Add command column (containing button) 
                    GridViewCommandColumn commandColumn = new GridViewCommandColumn();
                    commandColumn.Name = "Izbrisi";
                    commandColumn.HeaderText = "";
                    commandColumn.Width = 160;
                    commandColumn.DefaultText = "Izbrisi file";
                    commandColumn.UseDefaultText = true;
                    commandColumn.AutoSizeMode = BestFitColumnMode.SummaryRowCells;

                    radGridView2.Columns.Add(commandColumn);


                    radGridView2.CommandCellClick += new CommandCellClickEventHandler(radGridView1_Izbrisi);
                }
            }



            for (int i = 0; i < radGridView2.RowCount; i++)
            {
                if ((int)radGridView2.Rows[i].Cells["TipDokumenta"].Value == 1)
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
                radWaitingBar2.Text = "Učitavam podatke";
                radWaitingBar2.StartWaiting();
                radGridView2.Rows.Clear();
                radGridView1.DataSource = await Task.Run(() => Citaj(_idSluzbe));
                radWaitingBar2.StopWaiting();
                radWaitingBar2.Text = "";
            }
        }



        private void radButton3_Click(object sender, EventArgs e)
        {
            try
            {
                editirano = false;
                
                mainProtokolEntities.SaveChanges();
                //RadMessageBox.ThemeName = "Office2010Silver";
                //RadMessageBox.Show("Izmjene spašene");
                ChangeMouseOverState(radButton3, 0);
            }

            catch
            {
                RadMessageBox.ThemeName = "Office2010Silver";
                RadMessageBox.Show("Došlo je do greške!");
            }
        }


        private static void ChangeMouseOverState(RadButton target, int i)
        {
          
            if (i==1)
            {
                target.ButtonElement.IsMouseOver = true;
            }
            else
            {
                target.ButtonElement.IsMouseOver = false;
            }
            
        }

        private void radGridView1_CellEditorInitialized(object sender, GridViewCellEventArgs e)
        {
            if (e.ActiveEditor is MyAutoCompleteEditor)
            {
                using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
                {
                    var razvodAutocomplete = context.protokol.Where(r=>r.ID_sluzbe ==_idSluzbe).Select(r => r.razvod).Distinct().ToList();
                    MyAutoCompleteEditor editor = (MyAutoCompleteEditor)e.ActiveEditor;
                    RadAutoCompleteBoxElement element = (RadAutoCompleteBoxElement)editor.EditorElement;
                    element.Delimiter = ' ';
                    element.AutoCompleteDataSource = razvodAutocomplete;
                }

            }
            editirano = true;
            
            ChangeMouseOverState(radButton3, 1);
        }

        private void RadForm1_FormClosing(object sender, FormClosingEventArgs e)
        {
           RadMessageBox.ThemeName = "Office2010Silver";
           if (Helper.CanEdit(Environment.UserName) && editirano)
           {
               if (RadMessageBox.Show("Da li zelite spasiti izmjene?", "Pitanje", MessageBoxButtons.YesNo) == DialogResult.Yes)
               {
                   radButton3.PerformClick();
               }
            }

           try
           {
               using (XmlWriter w = XmlWriter.Create(Directory.GetCurrentDirectory() + "\\MainLayout.xml"))
               {
                   radGridView1.SaveLayout(w);

               }
            }
            catch { }

        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            if (radGridView1.CurrentRow.Cells["ID"].Value != null)
            {
                int idProtokola = (int)radGridView1.CurrentRow.Cells["ID"].Value;
                UnosFileova unos = new UnosFileova(idProtokola, this, (string)radGridView1.CurrentRow.Cells["predmet"].Value, (string)radGridView1.CurrentRow.Cells["veza"].Value);
                radGroupBox2.Enabled = false;
                unos.Show();
            }
        }

        private  void radGridView1_EnabledChanged(object sender, EventArgs e)
        {
            try
            {
                if (radGridView1.CurrentRow.Cells["ID"].Value != null)
                {
                    int idProtokola = (int) radGridView1.CurrentRow.Cells["ID"].Value;

                    radGridView2.DataSource = CitajDokumentList(idProtokola);
                }
            }

            catch
            {
            }

        }

        #region SaveButton
        private void radButton4_Click(object sender, EventArgs e)
        {
            RadMessageBox.ThemeName = "Office2010Silver";
            if (Helper.CanEdit(Environment.UserName)&&editirano)
            {
                if (RadMessageBox.Show("Da li zelite spasiti izmjene?", "Pitanje", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    radButton3.PerformClick();
                }
            }
            try
            {
                if (radGridView1.CurrentRow.Cells["ID"].Value != null)
                {
                    int idProtokola = (int)radGridView1.CurrentRow.Cells["ID"].Value;
                    UnosFileova unos = new UnosFileova(idProtokola, this, (string)radGridView1.CurrentRow.Cells["predmet"].Value, (string)radGridView1.CurrentRow.Cells["veza"].Value);
                    radGroupBox2.Enabled = false;
                    unos.Show();
                }

            }
            catch (Exception exception)
            {
                RadMessageBox.SetThemeName("Office2010Silver");
                RadMessageBox.Show("Nije selektiran niti jedan unos!");
            }

        }
#endregion
        private async void radDropDownList1_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            radWaitingBar2.Text = "Učitavam podatke";
            radWaitingBar2.StartWaiting();
             _idSluzbe = (int) radDropDownList1.SelectedItem.Value;
             radGridView2.Rows.Clear();
             this.Text = "Protokol - " + Environment.UserName + " - " + radDropDownList1.SelectedItem.Text;
            radGridView1.DataSource = await Task.Run(() => Citaj(_idSluzbe));
            radWaitingBar2.Text = "";
            radWaitingBar2.StopWaiting();
        }

 #region exportExcel
        private string putanja;
        private async void radButton1_Click_2(object sender, EventArgs e)
        {
            RadSaveFileDialog fileDialog = new RadSaveFileDialog();
            fileDialog.SaveFileDialogForm.ThemeName = "Office2010Silver";
            fileDialog.DefaultExt = "xlsx";
            fileDialog.Filter = "*.xlsx | *.xlsx";
           
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                
                putanja = fileDialog.FileName;
                radWaitingBar1.StartWaiting();
                radWaitingBar1.Visible = true;
                var _progress = new Progress<string>();
                _progress.ProgressChanged += (objc, sndr) => { lblExportExcel.Text=sndr; };
                
                await this.ExportGridVisuallyAsync(_progress);
                radWaitingBar1.StopWaiting();
                radWaitingBar1.Visible = false;
                RadMessageBox.ThemeName = "Office2010Silver";
                if (RadMessageBox.Show("Export zavrsen! Zelite li vidjeti rezultirajuci file?", "Pitanje", MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
                    Process.Start(putanja);
                }

                lblExportExcel.Text = "";
            }


        }


        private async Task ExportGridVisuallyAsync(IProgress<string> progress = null)
        {

            var _progress = new Progress<string>();
            _progress.ProgressChanged += (ObjEct, SendEr) => { progress.Report(SendEr); };
            await Task.Run(() => ExportData(_progress));

        }

        public void ExportData(IProgress<string> progress = null)
        {
            progress.Report("Pocinjem");
            
            using (protokolEntities1 pro = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                progress.Report("Ucitavam podatke...");
                var protokolList = pro.protokol.Where(pr => pr.ID_sluzbe == _idSluzbe && pr.izbrisan == 0).OrderByDescending(p => p.redni_broj).ToList();
                progress.Report("Kreiram excel");
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(putanja) + "\\files"))
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(putanja) + "\\files");
                }
                using (var book = new XLWorkbook(XLEventTracking.Disabled))
                {
                    var sheet = book.Worksheets.Add("Protokol");

                    sheet.Cell(1, 1).Value = "Redni broj";
                    sheet.Cell(1, 2).Value = "Datum";
                    sheet.Cell(1, 3).Value = "Datum distribucije";
                    sheet.Cell(1, 4).Value = "Predmet";
                    sheet.Cell(1, 5).Value = "Oznaka dopisa";
                    sheet.Cell(1, 6).Value = "Dostava dopisa";
                    sheet.Cell(1, 7).Value = "Napomena";
                    sheet.Cell(1, 8).Value = "Tip dokumenta";
                    sheet.Cell(1, 9).Value = "Arhiva";
                    sheet.Cell(1, 10).Value = "Oznaka registratora";
                    sheet.Cell(1, 11).Value = "Razvod";
                    sheet.Cell(1, 12).Value = "Veza";

                    int brojreda = 2;
                    foreach (protokol p in protokolList)
                    {
                        sheet.Cell(brojreda, 1).Value = p.redni_broj;
                        sheet.Cell(brojreda, 2).Value = p.datum;
                        sheet.Cell(brojreda, 3).Value = p.datum_distribucije;
                        sheet.Cell(brojreda, 4).Value = p.predmet;
                        sheet.Cell(brojreda, 5).Value = p.oznaka_dopisa;
                        sheet.Cell(brojreda, 6).Value = p.dostava_dopisa;
                        sheet.Cell(brojreda, 7).Value = p.napomena;
                        if (p.ID_tipa == 0)
                        {
                            sheet.Cell(brojreda, 8).Value = "Bez tipa";
                        }

                        if (p.ID_tipa == 1)
                        {
                            sheet.Cell(brojreda, 8).Value = "Ulaz";
                        }

                        if (p.ID_tipa == 2)
                        {
                            sheet.Cell(brojreda, 8).Value = "Izlaz";
                        }

                        if (p.ID_tipa == 3)
                        {
                            sheet.Cell(brojreda, 8).Value = "Ulaz / izlaz";
                        }

                        sheet.Cell(brojreda, 9).Value = p.arhiva;
                        sheet.Cell(brojreda, 10).Value = p.oznaka_registratora;
                        sheet.Cell(brojreda, 11).Value = p.razvod;
                        sheet.Cell(brojreda, 12).Value = p.veza;

                        progress.Report("Spasavam fileove i kreiram linkove...");
                        
                        var _dokumenti = p.dokument.Where(d => d.Izbrisan == 0).Take(p.dokument.Count);

                        //Tipicni slucaj do dva neizbrisana filea, veza i predmet
                        if (_dokumenti.Count() <=2)
                        {
                            for (int i = 0; i < _dokumenti.Where(d => d.Izbrisan == 0).Count(); i++)
                            {
                                if (_dokumenti.ElementAt(i).Izbrisan == 0)
                                {
                                    if (i == 0)
                                    {
                                        databaseFileRead(_dokumenti.ElementAt(i).ID, System.IO.Path.GetDirectoryName(putanja) + "\\files\\" + _dokumenti.ElementAt(i).Filename);
                                        if (sheet.Cell(brojreda, 4).Value.ToString().Length > 0)
                                        {
                                            sheet.Cell(brojreda, 4).Hyperlink = new XLHyperlink(@"./files/" + _dokumenti.ElementAt(i).Filename);
                                        }
                                        else
                                        {
                                            sheet.Cell(brojreda, 4).Value = _dokumenti.ElementAt(i).Filename;
                                            sheet.Cell(brojreda, 4).Hyperlink = new XLHyperlink(@"./files/" + _dokumenti.ElementAt(i).Filename);
                                        }
                                        
                                        
                                    }

                                    if (i == 1)
                                    {
                                        databaseFileRead(_dokumenti.ElementAt(i).ID, System.IO.Path.GetDirectoryName(putanja) + "\\files\\" + _dokumenti.ElementAt(i).Filename);
                                        if (sheet.Cell(brojreda, 12).Value.ToString().Length > 0)
                                        {
                                            sheet.Cell(brojreda, 12).Hyperlink = new XLHyperlink(@"./files/" + _dokumenti.ElementAt(i).Filename);
                                        }
                                        else
                                        {
                                            sheet.Cell(brojreda, 12).Value = _dokumenti.ElementAt(i).Filename;
                                            sheet.Cell(brojreda, 12).Hyperlink = new XLHyperlink(@"./files/" + _dokumenti.ElementAt(i).Filename);
                                        }
                                        
                                    }


                                }

                            }

                        }
                        else
                        {
                            int lastColNumber = sheet.LastCell().Address.ColumnNumber;
                            //Ne znamo koliko dokumenata ima, ukoliko ode preko broja kolona - ide u paprikas
                            for (int i = 0; i < _dokumenti.Count(); i++)
                            {
                                if (i <= lastColNumber)
                                {
                                    if (_dokumenti.ElementAt(i).Izbrisan == 0)
                                    {
                                        databaseFileRead(_dokumenti.ElementAt(i).ID, System.IO.Path.GetDirectoryName(putanja) + "\\files\\" + _dokumenti.ElementAt(i).Filename);
                                        
                                        if (sheet.Cell(brojreda, i + 4).Value.ToString().Length > 0)
                                        {
                                            sheet.Cell(brojreda, i + 4).Hyperlink = new XLHyperlink(@"./files/" + _dokumenti.ElementAt(i).Filename);
                                        }
                                        else
                                        {
                                            sheet.Cell(brojreda, i + 4).Value = _dokumenti.ElementAt(i).Filename;
                                            sheet.Cell(brojreda, i + 4).Hyperlink = new XLHyperlink(@"./files/" + _dokumenti.ElementAt(i).Filename);
                                        }
                                        
                                    }
                                }
                                #region paprikas
                                #endregion


                            }

                        }
                    


                        brojreda++;
                    }
                    progress.Report("Formatiram Excel file...");
                    sheet.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    sheet.Column(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    sheet.ColumnsUsed().AdjustToContents();


                    var tabela = sheet.RangeUsed().CreateTable();

                    tabela.Theme = XLTableTheme.TableStyleMedium14;

                    book.SaveAs(putanja);
                    progress.Report("Zavrseno");
                }

            }

        }
#endregion

        void radGridView1_EditorRequired(object sender, Telerik.WinControls.UI.EditorRequiredEventArgs e)
        {
            if (radGridView1.CurrentColumn.Name == "razvod")
            {
                e.Editor = new MyAutoCompleteEditor();
            }
        }

    }
}

