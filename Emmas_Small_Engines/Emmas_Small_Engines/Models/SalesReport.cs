using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Xml.Linq;

namespace Emmas_Small_Engines.Models
{
    public class SalesReport
    {
        public int ID { get; set; }

        [Display(Name = "Report Name")]
        [Required(ErrorMessage = "The Sales Report Name cannot be empty.")]
        [StringLength(50, ErrorMessage = "The Sales Report Name cannot be more than 50 characters long.")]
        public string srepName { get; set; }

        [Display(Name = "Date Start")]
        [Required(ErrorMessage = "You must enter the Start Date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Date End")]
        [Required(ErrorMessage = "You must enter the End Date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Criteria")]
        //[Required(ErrorMessage = "The Sales Report Criteria cannot be empty.")]
        [StringLength(100, ErrorMessage = "The Sales Report Criteria cannot be more than 100 characters long.")]
        public string srepCriteria { get; set; }

        [Display(Name = "Cash")]
        [DataType(DataType.Currency)]
        public double Cash { get; set; }

        [Display(Name = "Debit")]
        [DataType(DataType.Currency)]
        public double Debit { get; set; }

        [Display(Name = "Credit")]
        [DataType(DataType.Currency)]
        public double Credit { get; set; }

        [Display(Name = "Cheque")]
        [DataType(DataType.Currency)]
        public double Cheque { get; set; }

        [Display(Name = "Payment Total")]
        [DataType(DataType.Currency)]
        public double paymentTotal { get; set; }

        [Display(Name = "Sale Tax (13%)")]
        [DataType(DataType.Currency)]
        public double SaleTax { get; set; }

        [Display(Name = "Other Taxes")]
        [DataType(DataType.Currency)]
        public double OtherTax { get; set; }

        [Display(Name = "Total Taxes")]
        [DataType(DataType.Currency)]
        public double TotalTax { get; set; }

        [Display(Name = "Appreciation Total")]
        [DataType(DataType.Currency)]
        public double AppreciationTotal { get; set; }

        [Display(Name = "Date of creation")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        [Display(Name = "Sales Employees")]
        public ICollection<SalesReportEmp> SalesReportEmps { get; set; } = new HashSet<SalesReportEmp>();

        [Display(Name = "Sales Items")]
        public ICollection<SalesReportItem> SalesReportItems { get; set; } = new HashSet<SalesReportItem>();

    }
}
