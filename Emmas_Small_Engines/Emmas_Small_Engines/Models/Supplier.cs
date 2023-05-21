using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
/*Written by Jia Ni Zhao
 On Jan 19, 2022*/

namespace Emmas_Small_Engines.Models
{
    public class Supplier
    {
        [Display(Name = "Supplier ID")]
        public int ID { get; set; }

        //format phone number
        [Display(Name = "Phone")]
        public string PhoneFormatted
        {
			get 
            {
                if (Phone != null && Phone.Length == 10) { return Phone.Substring(0, 3) + "-" + Phone.Substring(3, 3) + "-" + Phone[6..];
                } 
                else 
                { 
                    return "";
                }
            }
		}

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot be more than 50 characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50, ErrorMessage = "Address cannot be more than 50 characters long.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City cannot be more than 50 characters long.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Province is required.")]
        [StringLength(50, ErrorMessage = "Province cannot be more than 50 characters long.")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Postal code is required.")]
        [RegularExpression("^[A-Z]\\d[A-Z]\\d[A-Z]\\d$", ErrorMessage = "Postal code should be in Canadian format.")]
        [StringLength(6)]
        public string Postal { get; set; }

        public ICollection<Price> Prices { get; set; } = new HashSet<Price>();
    }
}
