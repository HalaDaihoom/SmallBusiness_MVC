using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusiness.Models
{
    public class Cart
    {
        [Key]
        [Display(Name = "ID")]
        public int CartId { get; set; }

        [Required(ErrorMessage = "Cart Quantity is required")]
        [Display(Name = "Quantity")]
        public int CartQuantity { get; set; }

        [Display(Name = "Price")]
        public decimal CartPrice { get; set; }


        public int UserID { get; set; }
        public  User User { get; set; }


        public virtual ICollection<OrderItem> OrderItems { get; set; }
        
    }
}
