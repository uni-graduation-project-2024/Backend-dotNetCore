using Learntendo_backend.Data;
using Learntendo_backend.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Learntendo_backend.Dtos;
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;
using Microsoft.AspNetCore.SignalR;
using System.Reflection.PortableExecutable;
using iText.Kernel.Pdf;
using System;
using Microsoft.EntityFrameworkCore;

namespace Learntendo_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : Controller
    {

        private readonly IDataRepository<Files> _filerepo;
        private readonly IDataRepository<User> _userrepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public FilesController(IDataRepository<Files> filerepo, IDataRepository<User> userrepo, IMapper mapper , IWebHostEnvironment env , DataContext context)
        {
            _env = env;
            _filerepo = filerepo;
            _mapper = mapper;
            _userrepo = userrepo;
            _context = context;
        }
       

        //upload function
        [HttpPost("Upload")]
        public async Task<IActionResult> UploadFile([FromForm] FilesDto file)
        {
            if (file == null || file.File.Length == 0)
            {
                return BadRequest("Please Upload The File");
            }

            const long maxFileSize = 5 * 1024 * 1024; // 5MB

            if (file.File.Length > maxFileSize)
            {
                return BadRequest($"File size exceeds the allowed limit of 5MB.");
            }
            int maxPages = 50;
            using (var stream = file.File.OpenReadStream())
            {
                using (var pdfReader = new PdfReader(stream))
                {
                    using (var pdfDocument = new PdfDocument(pdfReader))
                    {
                        int totalPages = pdfDocument.GetNumberOfPages();
                        if (totalPages > maxPages)
                        {
                            return BadRequest($"The file contains {totalPages} pages, exceeding the limit of {maxPages} pages.");
                        }
                    }
                }
            }
            var filefolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(filefolder))
            {
                Directory.CreateDirectory(filefolder);
            }
            //if you need uniqename
            var uniqueFileName = $"{Guid.NewGuid()}_{file.File.FileName}";

            var filePath = Path.Combine(filefolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.File.CopyToAsync(stream);
            }

            var fileRecord = new Files
            {
                FileName = file.File.FileName,
                FilePath = $"/uploads/{uniqueFileName}",
                CreatedDate = DateTime.Now,
                UserId = file.UserId
            };

            await _userrepo.ChecknumofgeneratedFile(file.UserId);
             

            await _filerepo.AddFun(fileRecord);

            return Ok(new { message = "File Uploaded Successfully.", path = fileRecord.FilePath });
         
        }

        //download function
        [HttpGet("download/{FileId}")]
        public async Task<IActionResult> Download(int FileId)
        {
            var fileRecord = await _filerepo.GetByIdFun(FileId);
            if (fileRecord?.FilePath == null)
            {
                return NotFound("File not found.");
            }

            var fileName = Path.GetFileName(fileRecord.FilePath); 
            var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"File not found: {filePath}");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", fileName);
        }


        [HttpGet("UserUploadFiles/{userId}")]
        public async Task<IActionResult> UserUploadFiles(int userId)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var filesdto = _mapper.Map<FilesDto>(user);
            return Ok(new
            {
                filesdto.UploadedFiles
            });
        }



    }
}
