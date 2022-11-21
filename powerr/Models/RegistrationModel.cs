using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace powerr.Models
{
    public class RegistrationModel
    {
        [Required(ErrorMessage ="First name is required")]
        [StringLength(50, ErrorMessage = "Should not be more than 50")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Should not be more than 50")]
        public string? LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage ="Email is required")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Password is required")]

        public string? Password { get; set; }
    }
}
