using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Emmas_Small_Engines.Models
{
    public class HourlyReport
    {
        public int ID { get; set; }

        [Display(Name = "Name")]
        public string hourName { get; set; }


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

        [Display(Name = "Hourly Report Criteria")]
        //[Required(ErrorMessage = "The Hourly Report Criteria cannot be empty.")]
        [StringLength(100, ErrorMessage = "The Hourly Report Criteria cannot be more than 100 characters long.")]
        public string hrepCriteria { get; set; }

        [Display(Name = "Billable Hours")]
        public double hour { get; set; }

        [Timestamp]
        public Byte[] RowVersion { get; set; }


        [Display(Name = "Employee Logins")]
        public ICollection<EmpLogin> EmpLogins { get; set; } = new HashSet<EmpLogin>();

        [Display(Name = "Date of creation")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}
