using Learntendo_backend.Data;
using Learntendo_backend.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Learntendo_backend.Dtos;
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace Learntendo_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : Controller
    {

        private readonly IDataRepository<Files> _filerepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public FilesController(IDataRepository<Files> filerepo, IMapper mapper , IWebHostEnvironment env)
        {
            _env = env;
            _filerepo = filerepo;
            _mapper = mapper;
        }
        //upload function
        [HttpPost("Upload")]
        public async Task<IActionResult> UploadFile([FromForm] FilesDto file)
        {
            if (file == null || file.File.Length == 0)
            {
                return BadRequest("Please Upload The File");
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
                FilePath = $"/uploads/{uniqueFileName}"
            };

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



    }
}
