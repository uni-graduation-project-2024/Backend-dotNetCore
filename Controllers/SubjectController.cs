using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Learntendo_backend.Data;
using Learntendo_backend.Models;
using Learntendo_backend.Dtos;
using System;
using AutoMapper;

namespace Learntendo_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : Controller
    {
        private readonly IDataRepository<Subject> _subjectRepo;
        private readonly IMapper _map;
        private readonly IDataRepository<Exam> _examRepo;
        public SubjectController(IDataRepository<Subject> subjectRepo, IMapper map , IDataRepository<Exam> examRepo)
        {
            _subjectRepo = subjectRepo;
            _map = map;
            _examRepo = examRepo;
        }

    
        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetAllSubjects(int userId)
        {
            var subjects = await _subjectRepo.GetAllsubbyUserFun(userId);

        
            //var subjectDto = _map.Map<IEnumerable<SubjectDto>>(subjects);

            return Ok(subjects);
        }

      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(int id )
        {
            var subject = await _subjectRepo.GetByIdFun(id);
            if (subject == null)
            {
                return NotFound($"subject with this id {id} not found");
            }

            
            var subjectDto = _map.Map<SubjectDto>(subject);
            return Ok(subjectDto);
        }

      
        [HttpPost("{userId}")]
        public async Task<IActionResult> AddSubject(int userId, SubjectDto subjectDto)
        {
            var subject = _map.Map<Subject>(subjectDto);
            subject.UserId = userId;
            await _subjectRepo.AddFun(subject);
            return Ok(subject) ;
        }

   
        [HttpPut("updateSubjectName")]
        public async Task<IActionResult> UpdateSubjectName(int id, string subjectNewName)
        {
            var subject = await _subjectRepo.GetByIdFun(id);
            if (subject == null)
            {
                return NotFound($"subject with this id {id} not found");
            }


            subject.SubjectName = subjectNewName;

            await _subjectRepo.UpdateFun(subject);
            return Ok(subjectNewName);
        }

        [HttpPut("updateSubjectColor")]
        public async Task<IActionResult> UpdateSubjectColor(int id, string subjectNewColor)
        {
            var subject = await _subjectRepo.GetByIdFun(id);
            if (subject == null)
            {
                return NotFound($"subject with this id {id} not found");
            }


            subject.SubjectColor = subjectNewColor;

            await _subjectRepo.UpdateFun(subject);
            return Ok(subjectNewColor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _subjectRepo.GetByIdFun(id);
            if (subject == null)
            {
                return NotFound($"Subject with ID {id} not found.");
            }

            var exams = await _examRepo.GetAllExambysubFun(id);

            if (exams.Any())
            {
                foreach (var exam in exams)
                {
                    await _examRepo.DeleteFun(exam.ExamId);
                }
            }

            await _subjectRepo.DeleteFun(id);

            return Ok("Deleted Successfully");
        }

    }

}


