using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ProtokolApp
{
    public partial class RadFormProtokolSP : Telerik.WinControls.UI.RadForm
    {
        public RadFormProtokolSP()
        {
            InitializeComponent();
           
        }

        private bool editirano = false;
        private protokolEntities1 mainProtokolEntities;
        private protokolEntities1 dokumentProtokolEntities;
       
        public async Task<BindingList<protokol>> Citaj(int id_sluzbe)
        {
            try
            {
                mainProtokolEntities = new protokolEntities1(Helper.ProcitajEntityConnectionString());
                await mainProtokolEntities.protokol.Include(s => s.dokument).Where(pr => pr.ID_sluzbe == id_sluzbe && pr.izbrisan == 0)
                        .OrderByDescending(ii => ii.ID).LoadAsync();

                return mainProtokolEntities.protokol.Local.ToBindingList();
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        private  async void RadForm2_Load(object sender, EventArgs e)
        {
            radGridView1.DataSource = await Task.Run(() => Citaj(6));
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            RadMessageBox.ThemeName = "Office2010Silver";
            if (Helper.CanEdit(Environment.UserName)&& editirano)
            {
                if (RadMessageBox.Show("Da li zelite spasiti izmjene?", "Pitanje", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    radButton3.PerformClick();
                }
            }
            radGridView1.MasterView.TableSearchRow.SuspendSearch();
            UnosSP unos = new UnosSP(this);
            this.Enabled = false;

            unos.Show();
        }

        private void radGridView1_CellEditorInitialized(object sender, Telerik.WinControls.UI.GridViewCellEventArgs e)
        {
            editirano = true;
            Helper.ChangeMouseOverState(radButton3, 1);
        }

        private void radButton3_Click(object sender, EventArgs e)
        {
            mainProtokolEntities.SaveChanges();
            editirano = false;
            Helper.ChangeMouseOverState(radButton3, 0);
        }
    }
}
