using System.ComponentModel.DataAnnotations;
/*Written by Jia Ni Zhao
 On Feb 17, 2022*/

namespace Emmas_Small_Engines.Models
{
    public class Price : IValidatableObject
    {
        public int ID { get; set; }

        [Display(Name = "Purchased Cost")]
        [Required(ErrorMessage = "You must enter the Purchased Cost.")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double Cost { get; set; }

        [Display(Name = "Purchased Date")]
        [Required(ErrorMessage = "You must enter the Purchased Date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]//dd/MM/yyyy
        public DateTime Date { get; set; }

        [Display(Name = "Count")]
        [Required(ErrorMessage = "Count be empty.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 0.")]
        public int Count { get; set; }

        [Required(ErrorMessage = "You must select a supplier.")]
        [Display(Name = "Supplier")]
        public int SupplierID { get; set; }

        public Supplier Supplier { get; set; }

        [Required(ErrorMessage = "You must select a UPC.")]
        [Display(Name = "UPC")]
        public string InventoryID { get; set; }

        [Display(Name = "UPC")]
        public Inventory Inventory { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date > DateTime.Today)
            {
                yield return new ValidationResult("Purchased date cannot be in the future.", new[] { "Date" });
            }
        }
    }
}
