using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.Models
{
    public class Review
    {
        [Key]
        [Display(Name = "ID")]
        public int ReviewId { get; set; }

        
        [Display(Name = "Comment")]
        public string ReviewComment { get; set; }



        [Display(Name = "Rate")]
        [Range(1, 5, ErrorMessage = "Star rating must be between 1 and 5")]
        public int ReviewRate { get; set; }

        

        public int UserId { get; set; }
        public virtual User User { get; set; }


        public int ItemId { get; set; }
        public virtual Item Item { get; set; }




    }
}
