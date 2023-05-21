using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Emmas_Small_Engines.Models
{
    public class SalesReportItem
    {
        public int ID { get; set; }

        [Display(Name = "Item Name")]
        [Required(ErrorMessage = "The Item Name cannot be empty.")]
        public string ItemName { get; set; }

        [Display(Name = "Quantity")]
        public int ItemQuantity { get; set; }

        [Display(Name = "Item Total")]
        [DataType(DataType.Currency)]
        public double ItemTotal { get; set; }

        [Display(Name = "Sales Report")]
        public int SalesReportID { get; set; }

        public SalesReport SalesReport { get; set; }
    }
}
