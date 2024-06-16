
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusiness.Models
{
    public class Item
    {
        internal readonly object Orders;

        [Key]
        [Display(Name = "ID")]
        public int ItemID { get; set; }

        [Required(ErrorMessage = "Item Name is required")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Display(Name = "Item Image")]
        public byte[]? ItemImage { get; set; }
        [NotMapped]
        public IFormFile clientFile { get; set; }

        [Required(ErrorMessage = "Item Price is required")]
        [Display(Name = "Item Price")]
        // the price of a single item 
        public decimal ItemPrice { get; set; }

        [Required(ErrorMessage = "Stock Amount is required")]
        [Display(Name = "Stock Amount")]
        public int StockAmount { get; set; }  // New property

        [Display(Name = "Description")]
        [DataType (DataType.Text)]
        public string? ItemDescription { get; set; }




        public int BrandID { get; set; }
        public virtual Brand Brand { get; set; }


        public virtual ICollection<Review> Reviews { get; set; }

       
        public virtual ICollection<OrderItem> ItemOrders { get; set; }
    }
}

