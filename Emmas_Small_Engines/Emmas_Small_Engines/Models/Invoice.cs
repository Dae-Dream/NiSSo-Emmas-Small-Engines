using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
/*Written by Jia Ni Zhao
 On Feb 17, 2022*/
namespace Emmas_Small_Engines.Models
{
    public class Invoice
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Date cannot be empty.")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot be more than 1000 characters long.")]
        public string Description { get; set; }

        [Display(Name = "Appreciation")]
        [Required(ErrorMessage = "Appreciation is required.")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        public double Appreciation { get; set; }

        [Display(Name = "Subtotal")]
        [Required(ErrorMessage = "You must enter the Subtotal.")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double Subtotal { get; set; }

        [Required(ErrorMessage = "You must select a Customer.")]
        [Display(Name = "Customer")]
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        [Required(ErrorMessage = "You must select a Employee.")]
        [Display(Name = "Employee")]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        //1:m Invoice and InvoiceLine  
        [Display(Name = "Invoice")]
        [ValidateNever]
        public ICollection<InvoiceLine> InvoiceLines { get; set; } = new HashSet<InvoiceLine>();

        [Display(Name = "Payment")]
        public ICollection<InvoicePayment> InvoicePayments { get; set; } = new HashSet<InvoicePayment>();
        public ICollection<COGSReport> COGSReports { get; set; } = new HashSet<COGSReport>();
    }
}
