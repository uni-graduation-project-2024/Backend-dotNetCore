using AutoMapper;
using Azure;
using Learntendo_backend.Data;
using Learntendo_backend.Dtos;
using Learntendo_backend.Dtos.Learntendo_backend.DTOs;
using Learntendo_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

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
        private readonly DataContext _db;

        public ExamController(IDataRepository<Subject> subjectRepo, IDataRepository<Exam> examRepo, IDataRepository<User> userRepo, IMapper map , DataContext db)
        {
            _userRepo = userRepo;
            _examRepo = examRepo;
            _subjectRepo = subjectRepo;
            _map = map;
            _db = db;
        }

        [HttpPut("UseOneGenerationPower/{userId}")]
        public async Task<IActionResult> UseOneGenerationPower(int userId)
        {
            var user = await _userRepo.GetByIdFun(userId);
            if (user == null)
            {
                return NotFound($"User with {userId} not found");
            }
            else if (user.GenerationPower > 0) //change NumFilesUploadedToday -> GenerationPower
            {
                user.GenerationPower -= 1;
            }
            else if (user.GenerationPower <= 0)
            {
                return NotFound("Sorry, You are out of generation power.");
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("RequestGenerateExam/{userId}")]
        public async Task<IActionResult> RequestGenerateExam(int userId)
        {
            var user = await _userRepo.GetByIdFun(userId);
            if (user == null)
            {
                return NotFound($"User with {userId} not found");
            }
            else if (user.GenerationPower <= 0) //change NumFilesUploadedToday -> GenerationPower
            {
                return NotFound();
            }
            else
            {
                return Ok();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] ExamDto examDto)
        {
            var today = DateTime.Today;

            int examsToday = await _db.Exam
                .Where(e => e.UserId == examDto.UserId && e.CreatedDate >= today)
                .CountAsync();

            var exam = _map.Map<Exam>(examDto);
            exam.TfQuestionsData = null;
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

        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetAllExamByUserId(int userId,[FromQuery] int? subId=null)
        {
            var exams = await _examRepo.GetAllExambyUserFun(userId, subId);

            var viewExamDto = _map.Map<IEnumerable<ViewExamDto>>(exams);
            return Ok(viewExamDto);
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

            if (subId == -1) // Adding case: Moving subject to No folder
            {
                exam.SubjectId = null;
                await _examRepo.UpdateFun(exam);
                return Ok("Exam moved to noFolder successfully.");
            }
            else
            {
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

        [HttpPut]
        public async Task<IActionResult> RetryExam([FromBody] ExamDto examDto)
        {
            var exam = _map.Map<Exam>(examDto);
            exam.TfQuestionsData = null;
            exam.CreatedDate = DateTime.Now; // UpdatedDate
            await _examRepo.UpdateFun(exam);
            await _examRepo.UpdatePostExamRelatedTable(exam.ExamId);
            return CreatedAtAction(nameof(GetExamById), new { id = exam.ExamId }, exam);

        }


    }
}
