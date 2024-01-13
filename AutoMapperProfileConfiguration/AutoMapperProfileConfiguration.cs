using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tangehrine.DataLayer.Model.Admin;
using Tangehrine.ServiceLayer.Dtos.Admin;

namespace  Integr8ed.AutoMapperProfileConfiguration
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            //CreateMap<Room_TypeDto, Room_Type>()
            //    .ForMember(des => des.Title, opt => opt.MapFrom(src => src.RoomTitle))
            //.ReverseMap();
          
            CreateMap<PatientMedicineDto, PatientMedicine>().ReverseMap();
            CreateMap<PatientMedicineOutputDto, PatientMedicine>().ReverseMap();
            CreateMap<PatientStatementTrendsDto, PatientStatementTrends>().ReverseMap();

            //CreateMap<SecurityDto, Security>().ReverseMap();
            //CreateMap<ContactDetailsDto, ContactDetails>().ReverseMap();
            //CreateMap<BookingDetailsDto, BookingDetails>().ReverseMap();
            //CreateMap<CallLogsDto, CallLogs>().ReverseMap();
        }
    }

    public static class MappingExpression
    {
        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(this IMappingExpression<TSource, TDestination> map, Expression<Func<TDestination, object>> selector)
        {
            map.ForMember(selector, config => config.Ignore());
            return map;
        }
    }

}
