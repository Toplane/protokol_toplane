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
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using Telerik.WinControls.UI.Localization;
using Telerik.WinControls.Export;
using Telerik.WinControls.UI.Export;
using ClosedXML.Excel;
using ProtokolApp.Klase;
using Telerik.WinControls.Data;
using TableDependency;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using EntityFramework.Triggers;


namespace ProtokolApp
{
    public partial class RadFormOsnovniProtokol : FormC
    {
        private static int _idSluzbe = 0;
        private static int _idKorisnika = 0;
        private bool editirano = false;
        private protokol _protokol;

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
        private BindingList<protokol> blProtokols = new BindingList<protokol>();

        
        void IzmjeneDokumentEventHandler(object sender, RecordChangedEventArgs<dokument> e)
        {
            if (blProtokols.Any(d => d.ID == e.Entity.ID_protokola))
            {
                switch (e.ChangeType)
                {
                    case ChangeType.Insert:
                        try
                        {
                            radGridView1.Invoke((MethodInvoker) delegate
                            {
                                var trenutniRed = radGridView2.CurrentRow;
                                mainProtokolEntities.dokument.Attach(e.Entity);

                                protokol entity = mainProtokolEntities.protokol.Find(e.Entity.ID_protokola);
                                mainProtokolEntities.Entry(entity).Reload();

                                FormatirajGrid();

                                radGridView2.CurrentRow = trenutniRed;


                            });

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }



                        break;


                    case ChangeType.Update:

                        radGridView1.Invoke((MethodInvoker) delegate
                        {
                            bool brisan = false;
                            var trenutniRed = radGridView2.CurrentRow;

                            protokol entity = mainProtokolEntities.protokol.Find(e.Entity.ID_protokola);
                            dokument entityDokument = mainProtokolEntities.dokument.Find(e.Entity.ID);
                            mainProtokolEntities.Entry(entity).Reload();
                            mainProtokolEntities.Entry(entityDokument).Reload();

                            if (e.EntityOldValues.Izbrisan == 0 && e.Entity.Izbrisan == 1)
                            {
                                mainProtokolEntities.Entry(entityDokument).State = EntityState.Detached;
                                radGridView2.DataSource = mainProtokolEntities.dokument.Local.ToBindingList();
                                brisan = true;
                                FormatirajGrid();

                            }

                            if (!brisan)
                            {
                                radGridView2.CurrentRow = trenutniRed;
                            }

                        });


                        break;

                    default:
                        break;
                        

                }
            }
        }

        private void TrackChanges(protokol old, protokol novi)
        {
            StringBuilder sb = new StringBuilder();
            string propertyName = novi.GetType().Name;
            int id = old.ID;
            int idNew = novi.ID;
            DateTime datumpromjene = DateTime.Now;

            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                
                for (int i = 0; i < novi.GetType().GetProperties().Length; i++)
                {
                    //Obje vrijednosti postoje, treba provjeriti da li su mijenjane
                    if (novi.GetType().GetProperties()[i].GetValue(novi, null) != null &&
                        old.GetType().GetProperties()[i].GetValue(old, null) != null)
                    {
                        if (novi.GetType().GetProperties()[i].GetValue(novi, null).ToString() !=
                            old.GetType().GetProperties()[i].GetValue(old, null).ToString())
                        {
                            sb.AppendLine(string.Format("Nova vrijednost polja {0} je {1}, stara vrijernost je {2}.",
                                 novi.GetType().GetProperties()[i].Name,
                                 novi.GetType().GetProperties()[i].GetValue(novi, null).ToString(),
                                 old.GetType().GetProperties()[i].GetValue(old, null)));
                        }

                    }
                    //Stara vrijednost je bila prazno polje
                    if (novi.GetType().GetProperties()[i].GetValue(novi, null) != null &&
                        (old.GetType().GetProperties()[i].GetValue(old, null) == null ||
                         old.GetType().GetProperties()[i].GetValue(old, null).ToString().Length == 0))
                    {
                        sb.AppendLine(string.Format("Nova vrijednost polja {0} je {1}, stara vrijernost je bila prazno polje.",
                            novi.GetType().GetProperties()[i].Name,
                            novi.GetType().GetProperties()[i].GetValue(novi, null)));
                    }

                    //Stara vrijednost je promijenjena u prazno polje
                    if (old.GetType().GetProperties()[i].GetValue(old, null) != null &&
                        (novi.GetType().GetProperties()[i].GetValue(novi, null) == null ||
                         novi.GetType().GetProperties()[i].GetValue(novi, null).ToString().Length == 0))
                    {
                        sb.AppendLine(string.Format("Stara vrijednost polja {0} je bila {1}, nova vrijernost je  prazno polje.",
                            old.GetType().GetProperties()[i].Name,
                            old.GetType().GetProperties()[i].GetValue(old, null)));
                    }
                }

                if (sb.ToString().Length > 0)
                {
                    log_promjena log = new log_promjena
                    {
                        Datum_promjene = datumpromjene,
                        ID_izvorni = id,
                        ID_izvorni_novi =idNew,
                        naziv_polja = propertyName,
                        Promjena = sb.ToString(),
                        korisnikapp = System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName,
                        imeracunara = System.Environment.MachineName,
                        ipAdresa = Helper.GetLocalIpAddress()

                    };


                    context.log_promjena.Add(log);
                    
                    context.SaveChanges();
                    trackChangesUpdateProtokol = false;
                }
            }

        }

        void IzmjeneProtokolEventHandler(object sender, RecordChangedEventArgs<protokol> e)
        {
           
            if (e.Entity.ID_sluzbe == (int)radDropDownList1.SelectedItem.Value)
            {
               
                switch (e.ChangeType)
                {
                    case ChangeType.Insert:
                        try
                        {
                            radGridView1.Invoke((MethodInvoker) delegate
                            {

                                var trenutniRed = radGridView1.CurrentRow;
                                mainProtokolEntities.protokol.Attach(e.Entity);
                                mainProtokolEntities.Entry(e.Entity).Reload();

                                if (radGridView1.GridViewElement.IsInEditMode)
                                {
                                    radGridView1.CurrentRow = trenutniRed;
                                    FormatirajGrid();
                                    return;
                                }

                                else
                                {
                                  //  radGridView1.TableElement.ScrollToRow(trenutniRed);
                                    FormatirajGrid();
                                }


                            });

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                       

                        break;


                    case ChangeType.Update:

                        radGridView1.Invoke((MethodInvoker) delegate
                        {

                            bool brisan = false;
                            var trenutniRed = radGridView1.CurrentRow;
                            var trenutniRedIndex = trenutniRed.Index;
                            protokol entity = mainProtokolEntities.protokol.Find(e.Entity.ID);
                            mainProtokolEntities.Entry(entity).Reload();
                            if (e.EntityOldValues.izbrisan == 0 && e.Entity.izbrisan == 1)
                            {
                                mainProtokolEntities.Entry(entity).State = EntityState.Detached;
                                FormatirajGrid();
                                brisan = true;
                                if (radGridView1.Rows.Count > 0)
                                {
                                    if (radGridView1 != null)
                                        radGridView1.CurrentRow = radGridView1.Rows.FirstOrDefault(d => d.Index == trenutniRedIndex - 1);
                                }
                                


                            }

                            if (!brisan)
                            {
                                radGridView1.CurrentRow = trenutniRed;
                            }

                            if (trackChangesUpdateProtokol)
                            {
                                TrackChanges(e.EntityOldValues, e.Entity);
                            }

                        });


                        break;


                    //case ChangeType.Delete:
                    //    goto case ChangeType.Update;
                    //    break;


                    case ChangeType.None:
                        break;
                    default:
                        break;
                        
                }

            }

        }

        private SqlTableDependency<protokol> depProtokol;
        private SqlTableDependency<dokument> depDokument;
        private bool trackChangesUpdateProtokol = false;
        private  void FrmOsnovni_Load(object sender, EventArgs e)
        {


            depProtokol = new SqlTableDependency<protokol>(Helper.ProcitajConnectionString(), null, null, null, null, null, DmlTriggerType.All, true, true);
            depProtokol.OnChanged += IzmjeneProtokolEventHandler;
            depProtokol.Start();

            depDokument = new SqlTableDependency<dokument>(Helper.ProcitajConnectionString(), null, null, null, null, null, DmlTriggerType.All, true, true);
            depDokument.OnChanged += IzmjeneDokumentEventHandler;
            depDokument.Start();

            Triggers<protokol>.Updated += j =>
            {
                trackChangesUpdateProtokol = true;
            };
            //radGridView1.EnablePaging = true;
            //radGridView1.PageSize = 200;
            try
            {
                _idKorisnika = Helper.GetIDKorisnika(Environment.UserName);
                //if (Helper.DaLiJeAktivan(Environment.UserName))
                //{
                //    RadMessageBox.Show("Korisnik je aktivan!");
                //    Environment.Exit(0);
                //}
                //Helper.UnesiInstancu(_idKorisnika);
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




        public async Task<BindingList<protokol>> Citaj(int id_sluzbe, DateTime? limitDateTime)
        {
            try
            {

                    mainProtokolEntities = new protokolEntities1(Helper.ProcitajEntityConnectionString());


                
                if (id_sluzbe != 6)
                {
                    //await mainProtokolEntities.protokol.Where(pr => pr.ID_sluzbe == id_sluzbe && pr.izbrisan == 0)
                    //    .OrderByDescending(ii => ii.ID).LoadAsync();
                    //await mainProtokolEntities.protokol.Include(s => s.dokument).Where(pr => pr.ID_sluzbe == id_sluzbe && pr.izbrisan == 0)
                    //    .OrderByDescending(ii => ii.ID).LoadAsync();
                    if (limitDateTime is null)
                    {
                        await mainProtokolEntities.protokol.Where(pr => pr.ID_sluzbe == id_sluzbe && pr.izbrisan == 0)
                            .OrderByDescending(ii => ii.ID).LoadAsync();
                    }
                    else
                    {
                        await mainProtokolEntities.protokol.Where(pr => pr.ID_sluzbe == id_sluzbe && pr.izbrisan == 0 && pr.datum >= limitDateTime)
                            .OrderByDescending(ii => ii.ID).LoadAsync();
                    }

                }
                else
                {
                    await mainProtokolEntities.protokol.Where(pr => pr.ID_sluzbe == id_sluzbe && pr.izbrisan == 0)
                        .OrderByDescending(ii => ii.ID).LoadAsync();
                    if (limitDateTime is null)
                    {
                        await mainProtokolEntities.protokol.Where(pr => pr.ID_sluzbe == id_sluzbe && pr.izbrisan == 0 && pr.datum >= limitDateTime)
                            .OrderByDescending(ii => ii.ID).LoadAsync();
                    }


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


        private void FormatirajGrid()
        {
            try
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
                    //  tipDokumentaColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
                    // tipDokumentaColumn.PropertyFilter = PropertyFilter.TextPrimitiveFilter;
                    //  tipDokumentaColumn.AllowFiltering = false;

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
                    if ((int) radGridView1.Rows[i].Cells["brojDokumenata"].Value > 0)
                    {
                        foreach (GridViewCellInfo c in radGridView1.Rows[i].Cells)
                        {

                            c.Style.ForeColor = Color.Blue;
                        }
                    }
                    else
                    {
                        foreach (GridViewCellInfo c in radGridView1.Rows[i].Cells)
                        {

                            c.Style.ForeColor = Color.Black;
                        }
                    }

                    try
                    {
                        radGridView1.Rows[i].Cells["ime"].Value = radGridView1.Rows[i].Cells["ID_tipa"].Value;
                    }
                    catch (Exception ex)
                    {
                        //Pada u slucaju da je filter ukljucen, null reference exception
                    }

                }


                radGridView1.MasterView.TableSearchRow.ResumeSearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }


        private void radGridView1_DataBindingComplete_1(object sender, GridViewBindingCompleteEventArgs e)
        {

            FormatirajGrid();

        }


        private void radGridView1_CellValueChanged(object sender, GridViewCellEventArgs e)
        {
            if (radGridView1.CurrentRow.Cells["ime"].Value != null)
            {
                if (radGridView1.CurrentRow.Index >= 0) //Nije header, header ima index -1;
                {
                    radGridView1.CurrentRow.Cells["ID_tipa"].Value = radGridView1.CurrentRow.Cells["ime"].Value;
                }
               
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
            UnosOsnovniProtokol unos = new UnosOsnovniProtokol(this, (int)radDropDownList1.SelectedItem.Value, _protokol);
            
            
            unos.Show();

        }

        void radGridView1_Izbrisi(object sender, GridViewCellEventArgs e)
        {
            if (Helper.CanEdit(Environment.UserName))
            {
                if (e.Column.Name == "Izbrisi")
                {
                    RadMessageBox.SetThemeName("Office2010Silver");
                    if (RadMessageBox.Show("Da li ste sigurni da želite izbrisati unos?", "Pitanje",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {

                        int id = 0;
                        //blProtokols = mainProtokolEntities.protokol.Local.ToBindingList();
                        //radGridView1.DataSource = blProtokols;
                        //  int.TryParse(radGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString(), out id);

                        int.TryParse(e.Row.Cells["id"].Value.ToString(), out id);

                        dokument _dokumentr = new dokument();

                        _dokumentr = mainProtokolEntities.dokument.Where(d => d.ID == id).FirstOrDefault();
                        _dokumentr.Izbrisan = 1;
                        mainProtokolEntities.SaveChanges();
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
                            blProtokols = mainProtokolEntities.protokol.Local.ToBindingList();
                            radGridView1.DataSource = blProtokols;
                          //  int.TryParse(radGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString(), out id);

                            int.TryParse(e.Row.Cells["id"].Value.ToString(), out id);

                            protokol _pr = new protokol();
                            
                            _pr = blProtokols.Where(d => d.ID == id).FirstOrDefault();
                            _pr.izbrisan = 1;
                            mainProtokolEntities.SaveChanges();

                            
                            //try
                            //{ //Nepotreban dio jer server vraca updatovanu listu
                            //    e.Row.Delete();
                            //}
                            //catch (Exception ex)
                            //{
                            //    MessageBox.Show(ex.ToString());
                            //}
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
                try
                {

                    int id = 0;
                int.TryParse(radGridView2.Rows[e.RowIndex].Cells["id"].Value.ToString(), out id);

                string putanjaDirektorija = Directory.GetCurrentDirectory() + "\\files\\";
                string imeFilea = radGridView2.Rows[e.RowIndex].Cells["Filename"].Value.ToString();


                string filename = putanjaDirektorija +
                                  imeFilea;

                    if (!Directory.Exists(putanjaDirektorija))
                    {
                        Directory.CreateDirectory(putanjaDirektorija);
                    }
                    databaseFileRead(id, filename);

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
              // radGridView1.DataSource = await Task.Run(() => Citaj(_idSluzbe, null));
                
                //mainProtokolEntities.protokol.Local.Add(bl.ElementAt(bl.Count() - 1));
                //radGridView1.DataSource = mainProtokolEntities.protokol.Local;
                ////if (this.p != null)
                ////{
                ////    try
                ////    {
                ////        mainProtokolEntities.protokol.Attach(this.p); //Novi unos u protokol
                ////    }
                ////    catch //Vec postoji, nije moguce attach
                ////    {
                ////        var entity = mainProtokolEntities.protokol.Find(p.ID);
                ////        if (entity != null)
                ////        {
                ////            mainProtokolEntities.Entry(entity).CurrentValues.SetValues(p); //Postojeci protokol, samo ga updateujemo
                ////        }
                ////    }
                    
                    
                ////   // Selektiraj(p.ID);
                ////    FormatirajGrid();
                    
                ////}

                radWaitingBar2.StopWaiting();
                radWaitingBar2.Text = "";
            }
        }

        private void Selektiraj(int id)
        {
            
            foreach (var row in radGridView1.Rows)
            {
                if ((int)row.Cells["ID"].Value == id)
                {
                    row.IsSelected = true;
                    row.IsCurrent = true;
                }
            }
            mainProtokolEntities.protokol.Local.OrderByDescending(d => d.ID).OrderByDescending(d => d.datum);

        }

        private void radButton3_Click(object sender, EventArgs e)
        {
            try
            {
                editirano = false;
                
                mainProtokolEntities.SaveChanges();
                //RadMessageBox.ThemeName = "Office2010Silver";
                //RadMessageBox.Show("IzmjeneProtokolEventHandler spašene");
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
            //if (e.ActiveEditor is MyAutoCompleteEditor)
            //{
            //    using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            //    {
            //        var razvodAutocomplete = context.protokol.Where(r=>r.ID_sluzbe ==_idSluzbe).Select(r => r.razvod).Distinct().ToList();
            //        MyAutoCompleteEditor editor = (MyAutoCompleteEditor)e.ActiveEditor;
            //        RadAutoCompleteBoxElement element = (RadAutoCompleteBoxElement)editor.EditorElement;
            //        element.Delimiter = ' ';
            //        element.AutoCompleteDataSource = razvodAutocomplete;
            //    }

            //}
            if (e.RowIndex != -1) // Filter row
            {

                editirano = true;
                ChangeMouseOverState(radButton3, 1);
            }
            
            
           
        }

        /// <summary>
        /// Vraca listu novih unosa u protokol gledajuci koji je zadnji unos
        /// </summary>
        /// <param name="newProtokols"></param>
        /// <param name="maxPrethodniId"></param>
        /// <param name="idSluzbe"></param>
        /// <returns></returns>
        private async Task OsvjeziListu(IProgress<List<protokol>> newProtokols, int maxPrethodniId, int idSluzbe)
        {
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                var newProtokol = context.protokol.Where(d => d.ID > maxPrethodniId && d.ID_sluzbe==idSluzbe && d.izbrisan==0).ToListAsync();
                var lista = Task.Run(() => newProtokol);
                var rezultatNovi = lista.Result;
                if (rezultatNovi.Any())
                {
                    newProtokols.Report(rezultatNovi);
                }


            }
        }
        List<int> zavrseniUpdate=new List<int>();
        /// <summary>
        /// Vraca listu updateovanih unosa u protokol, iskljucujuci vec one koji su u listi zavrsenih (po id unosu u promjene)
        /// </summary>
        /// <param name="updateovaniProtokoli"></param>
        /// <param name="idSluzbe"></param>
        /// <returns></returns>
        private async Task Updateovani(IProgress<List<protokol>> updateovaniProtokoli, int idSluzbe)
        {
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                var izmjene =
                    context.promjene.Where(d => d.promjena == 2).Select(d => d.id_izvorni).Distinct(); //lista izmjenjenih idijeva
                var zavrsenInts =
                    context.promjene.Where(d => d.promjena == 2 && !(zavrseniUpdate.Contains(d.ID))).Select(d => d.ID).Distinct();

                var IzmjeneProtokol = context.protokol.Where(d => izmjene.Contains(d.ID) && d.ID_sluzbe == idSluzbe && d.izbrisan == 0).ToListAsync();

                var listaIzmjena = Task.Run(() => IzmjeneProtokol);
                var rezultatIzmjene = listaIzmjena.Result;
                if (rezultatIzmjene.Any())
                {
                    zavrseniUpdate.AddRange(zavrsenInts);
                    updateovaniProtokoli.Report(rezultatIzmjene);

                }
            }
        }

        List<int> zavrseniIzbrisani = new List<int>();
        private async Task Izbrisani(IProgress<List<protokol>> izbrisaniProtokoli, int idSluzbe)
        {
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                
                var izmjene =
                    context.promjene.Where(d => d.promjena == 3).Select(d => d.id_izvorni).Distinct(); //lista izmjenjenih idijeva
                var zavrsenInts =
                    context.promjene.Where(d => d.promjena == 3 && !(zavrseniIzbrisani.Contains(d.ID))).Select(d => d.ID).Distinct();

                var IzmjeneProtokol = context.protokol.Where(d => izmjene.Contains(d.ID) && d.ID_sluzbe == idSluzbe && d.izbrisan == 0).ToListAsync();
            }
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
            depProtokol.Stop();
            depDokument.Stop();
           //Helper.IzbrisiSesije(Environment.UserName);

        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            if (radGridView1.CurrentRow.Cells["ID"].Value != null)
            {
                int idProtokola = (int)radGridView1.CurrentRow.Cells["ID"].Value;
                UnosFileova unos = new UnosFileova(idProtokola, this, (string)radGridView1.CurrentRow.Cells["predmet"].Value, (string)radGridView1.CurrentRow.Cells["veza"].Value, _protokol);
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
                    UnosFileova unos = new UnosFileova(idProtokola, this, (string)radGridView1.CurrentRow.Cells["predmet"].Value, (string)radGridView1.CurrentRow.Cells["veza"].Value, _protokol);
                  //  radGroupBox2.Enabled = false;
                    this.Enabled = false;
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
             try
             {

              // blProtokols = await Task.Run(() => Citaj(_idSluzbe,  null));
               radGridView1.DataSource = blProtokols = await Task.Run(() => Citaj(_idSluzbe, null));

            }
             catch (Exception ex)
             {

                  //   RadMessageBox.Show("Doslo je do greske, kontaktirajte administratora!");


             }

            
            radWaitingBar2.Text = "";
            radWaitingBar2.StopWaiting();
            try
            {
                if ((int)radDropDownList1.SelectedItem.Value == 9) //ID protokola racuni je 9
                {
                    foreach (GridViewColumn c in radGridView1.Columns)
                    {
                        switch (c.Name)
                        {
                            case "predmet":

                                c.HeaderText = "Broj računa";
                                break;

                            case "dostava_dopisa":
                                c.HeaderText= "Iznos";
                                break;

                            case "razvod":
                                c.HeaderText = "Pošiljalac";
                                break;

                            case "veza":
                                c.HeaderText = "Kome je signiran?";
                                break;
                        }

                    }

                }

                if ((int)radDropDownList1.SelectedItem.Value == 1)
                {
                    foreach (GridViewColumn c in radGridView1.Columns)
                    {
                        switch (c.Name)
                        {
                            case "predmet":

                                c.HeaderText = "Predmet";
                                break;

                            case "dostava_dopisa":
                                c.HeaderText = "Dostava dopisa";
                                break;

                            case "razvod":
                                c.HeaderText = "Razvod";
                                break;

                            case "veza":
                                c.HeaderText = "Veza";
                                break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }
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

        private void radGridView1_RowFormatting(object sender, RowFormattingEventArgs e)
        {


        }

        private void radButton5_Click(object sender, EventArgs e)
        {
            if (radGridView1.RowCount > 0)
            {
                foreach (GridViewRowInfo r in radGridView1.Rows)
                {
                    if (Helper.BrojDokumenata((int) r.Cells["ID"].Value) > 0)
                    {
                        foreach (GridViewCellInfo c in r.Cells)
                        {
                            c.Style.ForeColor = Color.Blue;
                        }
                       
                    }
                }
            }
        }

        private void radGridView1_FilterChanged(object sender, GridViewCollectionChangedEventArgs e)
        {

        }

        private void radButton5_Click_1(object sender, EventArgs e)
        {
        }

        private void radGroupBox1_Click(object sender, EventArgs e)
        {

        }

        private void radDropDownList2_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Progress<List<protokol>> novaLista = new Progress<List<protokol>>();
            int maxId = mainProtokolEntities.protokol.Local.Where(d => d.ID_sluzbe == _idSluzbe).Max(d => d.ID);
            novaLista.ProgressChanged += (o, s) =>
            {
                foreach (var p in s)
                {
                    mainProtokolEntities.protokol.Attach(p);
                }
                
                FormatirajGrid();
                Selektiraj(s.Max(d=>d.ID));
            };

            var izmjeneProgress = new Progress<List<protokol>>();

            izmjeneProgress.ProgressChanged += (o, s) =>
            {
                if (s.Any())
                {
                    foreach (var i in s)
                    {
                        var entity = mainProtokolEntities.protokol.Find(i.ID);
                        if (entity != null)
                        {
                            mainProtokolEntities.Entry(entity).Reload();
                           
                        }
                    }
                        blProtokols= mainProtokolEntities.protokol.Local.ToBindingList();
                        radGridView1.DataSource = blProtokols;
                        
                }

            };

            Task.Run(() => Updateovani(izmjeneProgress, _idSluzbe));
            Task.Run(() => OsvjeziListu(novaLista, maxId, _idSluzbe));

        }

        private void RadFormOsnovniProtokol_Activated(object sender, EventArgs e)
        {
        }

        private void RadFormOsnovniProtokol_Shown(object sender, EventArgs e)
        {
            
        }

        private void RadFormOsnovniProtokol_Resize(object sender, EventArgs e)
        {
            if ((WindowState == FormWindowState.Normal || WindowState == FormWindowState.Maximized))
            {
                radGridView1.TableElement.ScrollToRow(radGridView1.CurrentRow);
            }
        }

        private void MasterTemplate_UserAddedRow(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells["ID"].Value = Helper.GetNextMaxBrojProtokola(_idSluzbe, DateTime.Now.Year);
        }

        private void MasterTemplate_UserAddingRow(object sender, GridViewRowCancelEventArgs e)
        {

        }
    }
}

