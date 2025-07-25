using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations; // Required for [Required] and [Display] attributes in error fetching

namespace HostelMgtSystem.Models 
{
    public class RegistrationViewModel
    {
     
    
       
            public int? Id { get; set; }

            [Required(ErrorMessage = "User Name is required.")]
            [Display(Name = "Your Name")]
            public string UserName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please select a Hostel.")]
            [Display(Name = "Select Hostel")]
            public int? SelectedHostelId { get; set; }
            public List<SelectListItem> HostelOptions { get; set; } = new List<SelectListItem>();

       
            [Display(Name = "Select Room")]
            public int? SelectedRoomId { get; set; } // Nullable for pending rooms
            public List<SelectListItem> RoomOptions { get; set; } = new List<SelectListItem>();

        public bool IsApproved { get; set; } = false;
        }
    }
