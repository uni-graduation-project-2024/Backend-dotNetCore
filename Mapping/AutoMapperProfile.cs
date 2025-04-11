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

            CreateMap<Exam, ViewExamDto>();

            CreateMap<ViewExamDto, Exam>();

            CreateMap<User, UserNavbarInfoDto>();

            CreateMap<UserNavbarInfoDto, User>();

            CreateMap<User, UserDto>().ReverseMap();

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

            CreateMap<User, UserDto>()
            .ForMember(dest => dest.LeagueHistory, opt => opt.MapFrom(src =>
            !string.IsNullOrEmpty(src.LeagueHistory)
              ? JsonConvert.DeserializeObject<Dictionary<string, string>>(src.LeagueHistory)
              : new Dictionary<string, string>()));

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.LeagueHistory, opt => opt.MapFrom(src =>
                    src.LeagueHistory != null && src.LeagueHistory.Any()
                        ? JsonConvert.SerializeObject(src.LeagueHistory)
                        : null));

        }
    }

}
