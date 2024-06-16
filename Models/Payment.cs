using System;
using System.ComponentModel.DataAnnotations;

namespace SmallBusiness.Models
{
    public class Payment
    {
        [Key]
        [Display(Name = "ID")]
        public int PaymentID { get; set; }

        [Required(ErrorMessage = "Payment Type is required")]
        [Display(Name = "Type")]
        public string PaymentType { get; set; }

        [Display(Name = "Visa Number")]
        [DataType(DataType.CreditCard)]
        [VisaNumberRequired("PaymentType")]
        public string? VisaNo { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Total Price is required")]
        [Display(Name = "Total Price")]
        [RegularExpression(@"^\d+.?\d{0,2}$")]
        public decimal PaymentPrice { get; set; }

        public int OrderID { get; set; }
        public virtual Order Orders { get; set; }
    }

    public class VisaNumberRequiredAttribute : ValidationAttribute
    {
        private readonly string _paymentTypePropertyName;

        public VisaNumberRequiredAttribute(string paymentTypePropertyName)
        {
            _paymentTypePropertyName = paymentTypePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var paymentTypeProperty = validationContext.ObjectType.GetProperty(_paymentTypePropertyName);
            if (paymentTypeProperty == null)
                return new ValidationResult($"Unknown property: {_paymentTypePropertyName}");

            var paymentType = paymentTypeProperty.GetValue(validationContext.ObjectInstance, null) as string;
            if (paymentType == "Visa" && string.IsNullOrWhiteSpace(value as string))
            {
                return new ValidationResult("Visa number is required for Visa payment.");
            }

            return ValidationResult.Success;
        }
    }
}