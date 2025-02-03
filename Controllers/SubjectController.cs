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

        public SubjectController(IDataRepository<Subject> subjectRepo, IMapper map)
        {
            _subjectRepo = subjectRepo;
            _map = map;
        }

    
        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetAllSubjects(int userId)
        {
            var subjects = await _subjectRepo.GetAllsubbyUserFun(userId);

        
            var subjectDto = _map.Map<IEnumerable<SubjectDto>>(subjects);

            return Ok(subjectDto);
        }

      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(int id )
        {
            var subject = await _subjectRepo.GetSubbyUserFun(id);
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
            return Ok(subjectDto) ;
        }

   
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, SubjectDto subjectDto)
        {
            var subject = await _subjectRepo.GetByIdFun(id);
            if (subject == null)
            {
                return NotFound($"subject with this id {id} not found");
            }


            _map.Map(subjectDto, subject);

            await _subjectRepo.UpdateFun(subject);
            return Ok(subjectDto);
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _subjectRepo.GetByIdFun(id);
            if (subject == null)
            {
                return NotFound($"subject with this id {id} not found");
            }

            await _subjectRepo.DeleteFun(id);
            return Ok("Deleted Successfully");
        }
    }

}


