using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PojistovnaApp.Models
{
    // Třída pro uložení informací o pojištěnci
    public class Pojistenec
    {
        // Identifikátor pojištěného
        [Key]
        public int Id { get; set; }

        // Jméno pojištěného
        [Required(ErrorMessage = "Jméno je povinné.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Jméno musí být mezi 2 a 50 znaky dlouhé.")]
        [DataType(DataType.Text, ErrorMessage = "Jméno musí být řetězec znaků.")]
        [Display(Name = "Jméno")]
        public string Jmeno { get; set; } = "";

        //Přijmení pojištěného
        [Required(ErrorMessage = "Přijmení je povinné.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Přijmení musí být mezi 2 a 50 znaky dlouhé.")]
        [DataType(DataType.Text, ErrorMessage = "Přijmení musí být řetězec znaků.")]
        [Display(Name = "Přijmení")]
        public string Prijmeni { get; set; } = "";

        //Email pojištěného
        [Required(ErrorMessage = "Email je povinný.")]
        [EmailAddress(ErrorMessage = "Zadejte prosím platnou e-mailovou adresu.")]
        public string Email { get; set; } = "";

        // Telefonní číslo pojištěného
        [Required(ErrorMessage = "Telefoní číslo je povinné.")]
        [RegularExpression(@"^\+?\d{1,3}\s?(\d{3}\s?){2}\d{3}$", ErrorMessage = "Zadejte prosím telefonní číslo ve formátu +420123456789.")]
        [Display(Name = "Telefon")]
        public string TelefonniCislo { get; set; } = "";

        // Ulice a číslo popisné pojištěného
        [Required(ErrorMessage = "Ulice a číslo popisné je povinné.")]
        [StringLength(50, ErrorMessage = "Ulice a číslo popisné může být nejvíce 50 znaků dlouhá.")]
        [DataType(DataType.Text, ErrorMessage = "Ulice a číslo popisné musí být řetězec znaků.")]
        [Display(Name = "Ulice a číslo popisné")]
        public string UliceCisloPopisne { get; set; } = "";

        //Město pojištěného
        [Required(ErrorMessage = "Město je povinné.")]
        [StringLength(50, ErrorMessage = "Město může být nejvíce 50 znaků dlouhá.")]
        [DataType(DataType.Text, ErrorMessage = "Město musí být řetězec znaků.")]
        [Display(Name = "Město")]
        public string Mesto { get; set; } = "";

        //PSČ pojištěného
        [Required(ErrorMessage = "Poštovní směrovací číslo je povinné.")]
        [RegularExpression(@"^\d{3}\s\d{2}$", ErrorMessage = "Zadejte prosím Poštovní směrovací číslo ve formátu 123 45.")]
        [Display(Name = "PSČ")]
        public string PSC { get; set; } = "";
    }
}
