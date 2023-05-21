using System.ComponentModel.DataAnnotations;
/*Written by Jia Ni Zhao
On Feb 17, 2022*/

namespace Emmas_Small_Engines.Models
{
    public class InvoiceLine
    {
        public int InvoiceID { get; set; }
        public Invoice Invoice { get; set; }

        public string InventoryID   { get; set; }
        public Inventory Inventory { get; set; }

        [Required(ErrorMessage = "You must enter the Quantity.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 0.")]
        public int Quantity { get; set; }

        [Display(Name = "Sale Price")]
        [Required(ErrorMessage = "You must enter the Sale Price.")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double SalePrice { get; set; }
    }
}
