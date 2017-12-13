
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SouthChandlerCycling.Models
{
    public class User
    {
        public int ID { get; set; }

        // These fields are used for authorization only...
        public string Password { get; set; }
        public string Salt { get; set; }

        [StringLength(50)]
        [Display(Name = "Role")]
        public string Role { get; set; } //tbd what I do with this...

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Column("UserName")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }
        [Required]
        [StringLength(14, ErrorMessage = "Phone Number format (xxx) yyy-zzzz")]
        [Display(Name = "Phone #")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
    }
}
