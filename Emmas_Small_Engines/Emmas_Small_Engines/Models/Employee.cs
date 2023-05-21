using System.ComponentModel.DataAnnotations;
/*Written by Jia Ni Zhao
 On Feb 17, 2022*/
namespace Emmas_Small_Engines.Models
{
    public class Employee
    {
        public int ID { get; set; }

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

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot be more than 50 characters long.")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "Password cannot be more than 50 characters long.")]
        public string Password { get; set; }

        [Display(Name = "Invoice")]
        public ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();
        public ICollection<EmpLogin> EmpLogins { get; set; } = new HashSet<EmpLogin>();
        public ICollection<EmployeePosition> EmployeePositions { get; set; } = new HashSet<EmployeePosition>();
    }
}
