using AutoMapper;
using Azure;
using Learntendo_backend.Data;
using Learntendo_backend.Dtos;
using Learntendo_backend.Dtos.Learntendo_backend.DTOs;
using Learntendo_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Learntendo_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : Controller
    {
        private readonly IDataRepository<Exam> _examRepo;
        private readonly IDataRepository<Subject> _subjectRepo;
        private readonly IDataRepository<User> _userRepo;
        private readonly IMapper _map;
        public ExamController(IDataRepository<Subject> subjectRepo, IDataRepository<Exam> examRepo, IDataRepository<User> userRepo, IMapper map)
        {
            _userRepo = userRepo;
            _examRepo = examRepo;
            _subjectRepo = subjectRepo;
            _map = map;
        }


        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] ExamDto examDto)
        {
            var exam = _map.Map<Exam>(examDto);
            if (examDto.SubjectId == 0)
            {
                exam.SubjectId = null; 
            }
            if (examDto.QuestionType == "MCQ")
            {
                exam.TfQuestionsData = null;
            }
            if (examDto.QuestionType == "TF")
            {
                exam.McqQuestionsData = null;
            }
            exam.CreatedDate = DateTime.Now;    
            await _examRepo.AddFun(exam);
            await _examRepo.UpdatePostExamRelatedTable(exam.ExamId);
            return CreatedAtAction(nameof(GetExamById), new { id = exam.ExamId }, exam);
          
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExamById(int id)
        {
            var exam = await _examRepo.GetByIdFun(id);
            if(exam == null)
            {
                return NotFound($"Exam with {id} not found");
            }
            var examDto = _map.Map<ExamDto>(exam);
            return Ok(examDto);
        }
        [HttpGet("all/{subId}")]
        public async Task<IActionResult> GetAllExamBySubId(int subId)
        {
            var exams = await _examRepo.GetAllExambysubFun(subId);
  
            var examDto = _map.Map<IEnumerable<ExamDto>>(exams);
            return Ok(examDto);
        }
        [HttpDelete("deleteprogress/{id}")]
        public async Task<IActionResult> DeleteExamWithPrograss(int id)
        {
            var exam = await _examRepo.GetByIdFun(id);
            if (exam == null)
            {
                return NotFound($"Exam with {id} not found");
            }
            await _examRepo.UpdateDeleteExamWithProgressRelatedTable(id);
            await _examRepo.DeleteFun(id);
            return Ok("Exam Deleted Successfully");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var exam = await _examRepo.GetByIdFun(id);
            if (exam == null)
            {
                return NotFound($"Exam with {id} not found");
            }
            await _examRepo.UpdateDeleteExamRelatedTable(id);
            await _examRepo.DeleteFun(id);
            return Ok("Exam Deleted Successfully");
        }
        [HttpPatch("{id}/{subId}")]
        public async Task<IActionResult> MoveExamToAnotherSub(int subId, [FromRoute] int id)
        {
            var exam = await _examRepo.GetByIdFun(id);
            if (exam == null)
            {
                return NotFound($"Exam with ID {id} not found.");
            }

            if (exam.SubjectId != null)
            {
                var oldSubject = await _subjectRepo.GetByIdFun(exam.SubjectId);
                if (oldSubject == null)
                {
                    return NotFound($"Old Subject with ID {exam.SubjectId} not found.");
                }

                oldSubject.NumExams -= 1;
                oldSubject.TotalQuestions -= exam.NumQuestions;
                if (oldSubject.NumExams < 0 || oldSubject.TotalQuestions < 0)
                {
                    return BadRequest("The number of exams or number of TotalQuestions in the old subject cannot be negative.");
                }
                await _subjectRepo.UpdateFun(oldSubject);
            }

            var newSubject = await _subjectRepo.GetByIdFun(subId);
            if (newSubject == null)
            {
                return NotFound($"New Subject with ID {subId} not found.");
            }

            newSubject.NumExams += 1;
            newSubject.TotalQuestions += exam.NumQuestions;
            await _subjectRepo.UpdateFun(newSubject);

            exam.SubjectId = subId;
            await _examRepo.UpdateFun(exam);

            return Ok("Exam moved successfully.");
        }




    }
}
