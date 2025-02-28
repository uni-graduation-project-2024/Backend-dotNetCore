﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Learntendo_backend.Models
{
    public class Files
    {
        [Key]
        public int FileId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }

        [NotMapped]
        public IFormFile FileUp { get; set; }
    }
}
