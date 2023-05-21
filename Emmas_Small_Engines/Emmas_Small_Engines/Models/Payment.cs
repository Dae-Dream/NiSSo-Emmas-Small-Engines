using System.ComponentModel.DataAnnotations;
/*Written by Jia Ni Zhao
 On Feb 17, 2022*/
namespace Emmas_Small_Engines.Models
{
    public class Payment
    {
        public int ID { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessage = "You cannot leave the Type blank.")]
        [StringLength(100, ErrorMessage = "Type cannot be more than 100 characters long.")]
        public string Type { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "You cannot leave the Description blank.")]
        [StringLength(1000, ErrorMessage = "Description cannot be more than 1000 characters long.")]
        public string Description { get; set; }

        [Display(Name = "Payment")]
        public ICollection<InvoicePayment> InvoicePayments { get; set; } = new HashSet<InvoicePayment>();
    }
}
