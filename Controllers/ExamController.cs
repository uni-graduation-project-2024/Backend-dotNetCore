using AutoMapper;
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
        private readonly IDataRepository<Exam> _ExamRepo;
        private readonly IMapper _map;

        public ExamController(IDataRepository<Exam> examRepo, IMapper map)
        {
            _ExamRepo = examRepo;
            _map = map;
        }
        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] ExamDto examDto)
        {
            var exam = _map.Map<Exam>(examDto);
            if (examDto.QuestionType == "MCQ")
            {
                exam.TfQuestionsData = null;
            }
            if (examDto.QuestionType == "TF")
            {
                exam.McqQuestionsData = null;
            }
            await _ExamRepo.AddFun(exam);
            return CreatedAtAction(nameof(GetExamById), new { id = exam.ExamId }, exam);
            //return Ok("created Successfully");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExamById(int id)
        {
            var exam = await _ExamRepo.GetByIdFun(id);
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
            var exam = await _ExamRepo.GetAllExambysubFun(subId);
  
            var examDto = _map.Map<IEnumerable<ExamDto>>(exam);
            return Ok(examDto);
        }
    }
}
