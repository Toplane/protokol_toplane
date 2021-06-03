using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using  AutoMapper;
using Telerik.WinControls.UI;
using AutoMapper.Collection;
using AutoMapper.EquivalencyExpression;


namespace ProtokolApp
{
   public class protokolKabinetDTO
    {
        public int ID { get; set; }
        public int ID_sluzbe  { get; set; }
        public int redni_broj { get; set; }
        public DateTime? datum { get; set; }
        public DateTime? datum_distribucije { get; set; }
        public int ID_tipa { get; set; }
        public string predmet { get; set; }
        public string razvod { get; set; }
        public string podbroj_Kabinet { get; set; }
        public string posiljalac_Kabinet { get; set; }
        public string redniBroj_Kabinet { get; set; }
        public string veza { get; set; }
        public string napomena { get; set; }
        public ICollection<dokument> dokument2 { get; set; }


        public static BindingList<protokolKabinetDTO> GetAll(protokolEntities1 protokol)
        {
            BindingList<protokol> _protokol = new BindingList<protokol>();
            _protokol = protokol.protokol.Local.ToBindingList();

            var mapper = AutoMapperConfiguration.mapper;

           var _protokolDTOs=mapper.Map<BindingList<protokol>, BindingList<protokolKabinetDTO>>(_protokol);

            return _protokolDTOs;

        }

        public static bool SaveAll(protokolEntities1 entity, BindingList<protokolKabinetDTO> dto)
        {


            var mapper = AutoMapperConfiguration.mapper;

            mapper.Map<BindingList<protokolKabinetDTO>, BindingList<protokol>> (dto, entity.protokol.Local.ToBindingList());

            entity.SaveChanges();

            return true;

        }

        public static bool Save(protokolKabinetDTO protokolKabinetDto)
        {
            using (var context = new protokolEntities1(Helper.ProcitajEntityConnectionString()))
            {
                protokol p = new protokol();

                var mapper = AutoMapperConfiguration.mapper;

                mapper.Map<protokolKabinetDTO, protokol>(protokolKabinetDto, p);
                context.protokol.Add(p);
                context.SaveChanges();
            }

                return true;
        }
    }
}
