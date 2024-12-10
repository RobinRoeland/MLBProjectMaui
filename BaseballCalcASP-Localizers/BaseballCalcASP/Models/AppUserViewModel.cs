using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BaseballCalcASP.Models
{
    public class CreateAppUserViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The confirmation password differs from the password.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class EditAppUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Deleted")]
        public bool deleted { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class DeleteAppUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
    }
}