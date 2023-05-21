using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
/*Written by Jia Ni Zhao
On Feb 17, 2022*/
namespace Emmas_Small_Engines.Models
{
    public class OrderRequest
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "You must enter a description.")]
        [StringLength(1000, ErrorMessage = "Description cannot be more than 1000 characters long.")]
        public string Description { get; set; }

        [Display(Name = "Sent Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? SentDate { get; set; }

        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReceiveDate { get; set; }

        [Required(ErrorMessage = "You must enter an external order number.")]
        [Display(Name = "External Order Number")]
        [StringLength(50, ErrorMessage = "External order number cannot be more than 50 characters long.")]
        public string ExternalOrderNumber { get; set; }

        //[Required(ErrorMessage = "You must select a customer.")]
        [Display(Name ="Customer")]
        [Required]
        public int? CustomerID { get; set; }
        public Customer Customer { get; set; }  

        [Display(Name = "Items")]
        public ICollection<OrderRequest_Inventory> OrderRequest_Inventories { get; set; } = new HashSet<OrderRequest_Inventory>();
    }
}
