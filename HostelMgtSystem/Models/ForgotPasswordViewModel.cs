﻿using System.ComponentModel.DataAnnotations;

namespace HostelMgtSystem.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
    }
}
