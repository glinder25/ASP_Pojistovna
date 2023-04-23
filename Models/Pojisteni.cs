using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PojistovnaApp.Models
{
    // Výčtový typ pro typ pojištění
    public enum TypPojisteni
    {
        Zdravotní,
        Majetkové,
        Cestovní
    }

    // Třída pro ukládání informací o pojisteni
    public class Pojisteni
    {
        // Identifikátor pojištění
        [Key]
        public int Id { get; set; }

        // Druh pojištění
        [Required(ErrorMessage = "Vyberte druh pojištění")]
        [Display(Name = "Pojištění")]
        public TypPojisteni DruhPojisteni { get; set; }

        // Pojistná částka
        [Required(ErrorMessage = "Částka je povinná.")]
        [Range(1, 10000000, ErrorMessage = "Integerové číslo musí být v rozsahu od 1 do 10 000 000.")]
        [Display(Name = "Částka")]
        public int Castka { get; set; }

        // Předmět pojištění
        [Required(ErrorMessage = "Předmět pojištění je povinný.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Předmět pojištění musí být mezi 2 a 50 znaky dlouhé.")]
        [DataType(DataType.Text, ErrorMessage = "Předmět pojištění musí být řetězec znaků.")]
        [Display(Name = "Předmět pojištění")]
        public string PredmetPojisteni { get; set; } = "";

        // Datum začátku platnosti pojištění
        [Required(ErrorMessage = "Datum začátku pojištění je povinné.")]
        [Display(Name = "Platnost od")]
        [DataType(DataType.Date)]
        public DateTime DatumZacatku { get; set; }

        // Datum konce platnosti pojištění
        [Required(ErrorMessage = "Datum konce pojištění je povinné.")]
        [Display(Name = "Platnost do")]
        [DataType(DataType.Date)]
        public DateTime DatumKonce { get; set; }
        //Slouží k identifikaci pojištence
        public int PojistnikId { get; set; }
    }
}
