﻿using System.ComponentModel.DataAnnotations;

namespace UserAuthApi.dto
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Username { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 4)]
        public string Password { get; set; }

        public string Role { get; set; }
    }
}