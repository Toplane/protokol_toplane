using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Linq;
using Telerik.WinControls.UI;

namespace ProtokolApp
{
    public partial class FormProtokolSP : Telerik.WinControls.UI.RadForm
    {
        public FormProtokolSP()
        {
            ThemeResolutionService.ApplicationThemeName = "Office2010Silver";
        
            InitializeComponent();
           
        }
        private  string konekcija = Directory.GetCurrentDirectory() + "\\konekcija";
        private protokolEntities1 mainProtokolEntities;
        private protokolEntities1 dokumentProtokolEntities;
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
                    await mainProtokolEntities.protokol.Include(s => s.dokument).Where(pr => pr.ID_sluzbe == id_sluzbe && pr.izbrisan == 0)
                        .OrderByDescending(ii => ii.ID).LoadAsync();

                }

                return mainProtokolEntities.protokol.Local.ToBindingList();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void protokolBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private async  void FormProtokolSP_Load(object sender, EventArgs e)
        {
            radGridView1.DataSource = await Task.Run(() => Citaj(6));
        }

        private void radGridView1_DataBindingComplete(object sender, Telerik.WinControls.UI.GridViewBindingCompleteEventArgs e)
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

            for (int i = 0; i < radGridView1.RowCount; i++)
            {
                radGridView1.Rows[i].Cells["ime"].Value = radGridView1.Rows[i].Cells["ID_tipa"].Value;
            }

            radGridView1.MasterView.TableSearchRow.ResumeSearch();
        }
    }
}
