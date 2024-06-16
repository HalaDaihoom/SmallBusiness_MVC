using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallBusiness.Models
{
    public class User
    {
        [Key]
        [Display(Name ="ID")]
        public int UserID { get; set; }


        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        [MaxLength(20, ErrorMessage = "First Name can't exceed 20 characters")]
        [MinLength(3, ErrorMessage = "First Name can't be less than 3 characters")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        [MaxLength(20, ErrorMessage = "Last Name can't exceed 20 characters")]
        [MinLength(3, ErrorMessage = "Last Name can't be less than 3 characters")]

        public string LastName { get; set; }



        

        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string UserEmail { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password can't be less than 8 characters")]
         public string UserPassword { get; set; }

        [Required(ErrorMessage = "Password's confirmation is required")]
        [Display(Name = "Password's Confirmation")]
        [DataType(DataType.Password)]
        [Compare("UserPassword", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }



        [Required(ErrorMessage = "Phone Number is required")]
        [Display(Name = "Phone")]
        [MaxLength (11, ErrorMessage = "Phone Number can't exceed 11 characters")]
        [MinLength(11, ErrorMessage = "Phone Number can't be less than 11 characters")]
        public string UserPhone { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string UserGender { get; set; }

        [Display(Name = "Image")]
        public byte[]? UserImage { get; set; }

        [NotMapped]
        public IFormFile clientFile { get; set; }

        //add type owner or usr
        public string type { get; set; }
       



        public Cart Cart { get; set; }

        public virtual ICollection<Brand> Brands { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Order> Orders { get; set; }


    }
}
