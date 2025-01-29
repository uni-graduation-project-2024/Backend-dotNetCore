
//using Learntendo_backend.Data;
//using Learntendo_backend.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Learntendo_backend.Controllers
//    {
//        [Route("api/[controller]")]
//        [ApiController]
//        public class SubjectController : ControllerBase
//        {
//            private readonly DbContext _context;

//            public SubjectController(DbContext context)
//            {
//                _context = context;
//            }

//            // 🟢 إضافة مادة جديدة
//            [HttpPost("add")]
//            public async Task<IActionResult> AddSubject([FromBody] Subject subject)
//            {
//                if (subject == null)
//                {
//                    return BadRequest("Invalid subject data.");
//                }

//                // التأكد من أن المستخدم موجود
//                var user = await _context.User.FindAsync(subject.UserId);
//                if (user == null)
//                {
//                    return NotFound("User not found.");
//                }

//                _context.Subjects.Add(subject);
//                await _context.SaveChangesAsync();

//                return Ok(new { message = "Subject added successfully!", subject });
//            }

//            // 🟡 تحديث اسم المادة فقط (Rename)
//            [HttpPut("rename/{subjectId}")]
//            public async Task<IActionResult> RenameSubject(int subjectId, [FromBody] string newName)
//            {
//                var subject = await _context.Subjects.FindAsync(subjectId);
//                if (subject == null)
//                {
//                    return NotFound("Subject not found.");
//                }

//                subject.SubjectName = newName;
//                await _context.SaveChangesAsync();

//                return Ok(new { message = "Subject renamed successfully!", subject });
//            }

//            // 🔴 حذف مادة
//            [HttpDelete("delete/{subjectId}")]
//            public async Task<IActionResult> DeleteSubject(int subjectId)
//            {
//                var subject = await _context.Subjects.FindAsync(subjectId);
//                if (subject == null)
//                {
//                    return NotFound("Subject not found.");
//                }

//                _context.Subjects.Remove(subject);
//                await _context.SaveChangesAsync();

//                return Ok(new { message = "Subject deleted successfully!" });
//            }
//        }
//    }



