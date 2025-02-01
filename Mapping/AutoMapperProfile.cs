using Learntendo_backend.Dtos;
using Learntendo_backend.Models;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Learntendo_backend.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Subject, SubjectDto>();

            CreateMap<SubjectDto, Subject>();
        }
    }

}
