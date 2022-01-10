using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Collection;
using AutoMapper.EquivalencyExpression;

namespace ProtokolApp
{
    public static class AutoMapperConfiguration
    {
        public static IMapper mapper = Configure();

        private static IMapper Configure()
        {
            MapperConfiguration con = new MapperConfiguration(config =>
            {
                config.AddCollectionMappers();
                config.CreateMap<protokolKabinetDTO, protokol>()
                    .EqualityComparison((src, dst) => src.ID == dst.ID);
                //config.CreateMap<protokol, protokol>()
                //    .ForMember(x => x.ID, opt => opt.Ignore())
                //    .EqualityComparison((src, dst) => src.ID == dst.ID);
                //config.CreateMap<protokol, protokolKabinetDTO>();
                config.CreateMap<protokol, protokolKabinetDTO>()
                    .ForMember(m => m.dokument2, s => s.MapFrom(r => r.dokument));
            });
            IMapper mapper = con.CreateMapper();
            return mapper;
        }

    }
}
