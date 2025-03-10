﻿using System.ComponentModel.DataAnnotations;

namespace Sadegh_Identity_Sample.Models.DTO
{
    public class RegisterDto
    {
   
        public string FirstName {  get; set; }
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
