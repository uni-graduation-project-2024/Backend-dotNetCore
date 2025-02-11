using Learntendo_backend.Dtos;
using Learntendo_backend.Models;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Learntendo_backend.Dtos.Learntendo_backend.DTOs;
using Newtonsoft.Json;

namespace Learntendo_backend.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Subject, SubjectDto>();

            CreateMap<SubjectDto, Subject>();

            CreateMap<ExamDto, Exam>()
            .ForMember(dest => dest.McqQuestionsData, opt => opt.MapFrom(src =>
                src.McqQuestionsData != null ? JsonConvert.SerializeObject(src.McqQuestionsData) : null))
            .ForMember(dest => dest.TfQuestionsData, opt => opt.MapFrom(src =>
                src.TfQuestionsData != null ? JsonConvert.SerializeObject(src.TfQuestionsData) : null));

            CreateMap<Exam, ExamDto>()
                .ForMember(dest => dest.McqQuestionsData, opt => opt.MapFrom(src =>
                    src.McqQuestionsData != null ? JsonConvert.DeserializeObject<List<McqQuestionDto>>(src.McqQuestionsData) : null))
                .ForMember(dest => dest.TfQuestionsData, opt => opt.MapFrom(src =>
                    src.TfQuestionsData != null ? JsonConvert.DeserializeObject<List<TfQuestionDto>>(src.TfQuestionsData) : null));
        }
    }

}
