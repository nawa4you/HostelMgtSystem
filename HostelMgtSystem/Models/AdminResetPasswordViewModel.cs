using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HostelMgtSystem.Models
{
    public class AdminResetPasswordViewModel
    {
        [Required(ErrorMessage = "Please select a user.")]
        [Display(Name = "Select User to Reset Password")]
        public string SelectedUserEmail { get; set; } = string.Empty;

        public List<SelectListItem> UsersList { get; set; } = new List<SelectListItem>();

       
        public const string DefaultPassword = "123456"; // Define the default password here
    }
}
