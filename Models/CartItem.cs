using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SmallBusiness.Models

{
    public class CartItem
    {
        [Key]
        public int ItemIdCart { get; set; }

        public string ItemNameCart { get; set; }

        public int QuantityCart { get; set; }

        public decimal ItemPriceCart { get; set; }
    
}
}
