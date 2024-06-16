using SmallBusiness.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusiness.Models
{
    public class Brand
    {
        [Key]
        [Display(Name = "ID")]
        public int BrandID { get; set; }

        [Required(ErrorMessage = "Brand Name is required")]
        [Display(Name = "Brand Name")]
        public string BrandName { get; set; }

        [Required(ErrorMessage = "Brand Image is required")]
        [Display(Name = "Brand Image")]
        public byte[]? BrandImage { get; set; }

        [NotMapped]
        public IFormFile clientFile { get; set; }




        public int CatageryId { get; set; }
        public virtual Catagery Catagery { get; set; }


        public int UserID { get; set; }
        public virtual User User { get; set; }



        public virtual ICollection<Item> Items { get; set; }

        


    }
}
