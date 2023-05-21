using System.ComponentModel.DataAnnotations;
/*Written by Jia Ni Zhao
 On Jan 19, 2022*/

namespace Emmas_Small_Engines.Models
{
    public class Customer
    {
        [Display(Name = "Customer ID")] 
        public int ID { get; set; }

        //format phone number
        [Display(Name = "Phone")]
        public string PhoneFormatted
        {
            get { if (Phone != null && Phone.Length == 10) { return Phone.Substring(0, 3) + "-" + Phone.Substring(3, 3) + "-" + Phone[6..]; } else { return ""; } }
        }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot be more than 50 characters long.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50, ErrorMessage = "Address cannot be more than 50 characters long.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City cannot be more than 50 characters long.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Province is required.")]
        [StringLength(50, ErrorMessage = "Province cannot be more than 50 characters long.")]
        public string Province { get; set; }

        [Display(Name = "Postal Code")]
        [Required(ErrorMessage = "Postal code is required.")]
        [RegularExpression("^[A-Z]\\d[A-Z]\\d[A-Z]\\d$", ErrorMessage = "Postal code should be in Canadian format.")]
        [StringLength(6)]
        public string Postal { get; set; }

        [Display(Name = "Order")]
        public ICollection<OrderRequest> OrderRequests { get; set; } = new HashSet<OrderRequest>();

        [Display(Name = "Invoice")]
        public ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();
    }
}
