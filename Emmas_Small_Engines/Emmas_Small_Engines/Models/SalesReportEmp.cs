using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Emmas_Small_Engines.Models
{
    public class SalesReportEmp
    {
        public int ID { get; set; }

        [Display(Name = "Employee Name")]
        [Required(ErrorMessage = "The Employee Name cannot be empty.")]
        public string EmpName { get; set; }

        [Display(Name = "Employee Sales")]
        [DataType(DataType.Currency)]
        public double EmpSales { get; set; }

        [Display(Name = "Sales Report")]
        public int SaleReportsID { get; set; }

        public SalesReport SaleReports { get; set; }
    }
}
