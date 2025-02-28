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
            //return View(file);
        }


     

        //[HttpPost("upload")]
        //public async Task<IActionResult> Upload([FromForm] FileUploadDto filedto)
        //{
        //    if (filedto.file == null || filedto.file.Length == 0)
        //        return BadRequest("الملف غير موجود.");

        //    var filefolder = Path.Combine(_env.WebRootPath, "uploads");

        //    if (!Directory.Exists(filefolder))
        //    {
        //        Directory.CreateDirectory(filefolder);
        //    }

        //    //if you need uniqename
        //    //var uniqueFileName = $"{Guid.NewGuid()}_{filedto.file.FileName}";
        //    var filepath = Path.Combine(filefolder, filedto.file.FileName);


        //    using (var stream = new FileStream(filepath, FileMode.Create))
        //    {
        //        await filedto.file.CopyToAsync(stream);
        //    }


        //    var fileRecord = new Files
        //    {
        //        FileName = filedto.file.FileName,
        //        FilePath = $"/uploads/{filedto.file.FileName}",
        //        //$"/uploads/{uniqueFileName}"
        //        SubjectId = filedto.subid
        //    };

        //    await _filerepo.AddFun(fileRecord);

        //    return Ok(new { message = "تم تحميل الملف بنجاح.", path = fileRecord.FilePath });
        //}
    }
}
