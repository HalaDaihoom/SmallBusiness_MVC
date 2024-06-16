using SmallBusiness.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusiness.Models
{
    public class Catagery
    {
        [Key]
        [Display(Name = "ID")]
        public int CatageryId { get; set; }

        [Required(ErrorMessage = "Catagery Name is required")]
        [Display(Name = "Catagery Name")]
        public string CatageryName { get; set; }

        [Display(Name = "Catagery Image")]
        public byte[]? CatageryImage { get; set; }

        [NotMapped]
        public IFormFile clientFile { get; set; }


        public virtual ICollection<Brand> Brand { get; set; }

    }
}