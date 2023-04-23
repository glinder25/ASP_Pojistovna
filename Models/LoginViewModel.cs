using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PojistovnaApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email je povinný.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Heslo je povinné.")]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Display(Name = "Zapmatuj si mě?")]
        public bool RememberMe { get; set; }
    }
}
