using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PojistovnaApp.Models
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Email je povinný.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Heslo je povinné.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} musí být delší než {2} znaků", MinimumLength = 5)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        [Compare("Password", ErrorMessage = "Hesla se musí shodovat")]
        public string ConfirmPassword { get; set; }
    }
}
