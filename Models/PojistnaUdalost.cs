using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PojistovnaApp.Models
{
    // Třída pro ukládání informací o pojistné události
    public class PojistnaUdalost
    {
        // Identifikátor pojistné události
        [Key]
        public int Id { get; set; }

        // Datum a čas události
        [Required(ErrorMessage = "Datum a čas je povinný.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Datum a čas události")]
        public DateTime Datum { get; set; }

        // Popis události
        [Required(ErrorMessage = "Popis události je povinný.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Popis události musí být mezi 2 a 200 znaky dlouhé.")]
        [DataType(DataType.Text, ErrorMessage = "Popis události musí být řetězec znaků.")]
        [Display(Name = "Popis události")]
        public string Popis { get; set; } = "";

        // Částka poškození
        [Required(ErrorMessage = "Částka poškození je povinná.")]
        [Range(1, 10000000, ErrorMessage = "Částka musí být v rozsahu od 1 do 10 000 000.")]
        [Display(Name = "Částka poškození")]
        public int CastkaPoskozeni { get; set; }

        // Identifikátor pojištění, na které se událost vztahuje
        public int PojisteniId { get; set; }
        //public Pojisteni Pojisteni { get; set; }
    }
}
