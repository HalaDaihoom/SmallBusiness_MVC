using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.Models
{
    public class Order
    {
        [Key]
        [Display(Name = "ID")]
        public int OrderID { get; set; }

        [Display(Name = "Price")]

        // the price of an amount of the same item 
        public decimal OrderPrice { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Display(Name = "Quantity")]
        [MinLength(1)]
        [MaxLength(100)]
        public int OrderQuantity { get; set; }


        [Display(Name = "Data")]
        [DataType (DataType.Date)]
        public DateTime OrderData { get; set; }


        public int UserID { get; set; }
        public virtual User Users { get; set; }




        public virtual Payment Payments { get; set; }


        public virtual ICollection<OrderItem> OrderItems { get; set; }
        

    }
}
