using System.ComponentModel.DataAnnotations;
/*Written by Jia Ni Zhao
 On Jan 19, 2022*/

namespace Emmas_Small_Engines.Models
{
    public class Inventory
    {
        [Key]
        [Display(Name = "UPC")]
        [Required(ErrorMessage = "UPC is required.")]
        [RegularExpression("^\\d{8}$", ErrorMessage = "Please enter a valid 8-digit UPC (no spaces).")]
        [StringLength(8)]
        public string UPC { get; set; }

        //format UPC
        [Display(Name = "UPC")]
        public string UPCFormatted
        {
            get
            { 
                return UPC.Substring(0, 3) + "-" + UPC.Substring(3, 4) + "-" + UPC[7..];
            }
        }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be more than 100 characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Size is required.")]
        [StringLength(100, ErrorMessage = "Size cannot be more than 100 characters long.")]
        public string Size { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [StringLength(50, ErrorMessage = "Quantity cannot be more than 50 characters long.")]
        public string Quantity { get; set; }

        [Display(Name = "Cost (AVG)")]
        [Required(ErrorMessage = "Cost (AVG) is required.")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double AdjustPrice { get; set; }

        [Display(Name = "Price (Retail)")]
        [Required(ErrorMessage = "Price (Retail) is required.")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double MarkupPrice { get; set; }

        public string IsCurrent
        {
            get
            {
                if (Current)
                    return "Y";
                else 
                    return "N";
                //return Current ? "Y" : "N";
            }
        }

        [Required(ErrorMessage = "Current cannot be empty.")]
        public bool Current { get; set; }

        [Required(ErrorMessage = "Stock cannot be empty.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 0.")]
        public int Stock { get; set; }

        [Display(Name = "Price")]
        public ICollection<Price> Prices { get; set; } = new HashSet<Price>();

        //1:m Inventory and InvoiceLine 
        [Display(Name = "Invoice")]
        public ICollection<InvoiceLine> InvoiceLines { get; set; } = new HashSet<InvoiceLine>();

        [Display(Name = "Request")]
        public ICollection<OrderRequest_Inventory> OrderRequest_Inventories { get; set; } = new HashSet<OrderRequest_Inventory>();

        public ICollection<COGSReport> COGSReports { get; set; } = new HashSet<COGSReport>();
    }
}
