﻿using System.ComponentModel.DataAnnotations;

namespace Learntendo_backend.Models
{
    public class Admin
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public required string Email { get; set; }

       
        [Required]
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }

        

            
      

    }
}
