using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Emmas_Small_Engines.Models
{
    public class EmpLogin
    {
        public int ID { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime WorkDate { get; set; }


        [Display(Name = "Sign In")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime SignIn { get; set; }

        [Display(Name = "Sign Out")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime SignOut { get; set; }

        public double Hours
        {
            get
            {
                TimeSpan timeDiff = SignOut - SignIn;
                double hours = System.Math.Abs(timeDiff.TotalHours);
                return hours;
            }
        }

        public int EmployeeID { get; set; }

        public Employee Employee { get; set; }

        public int? HourlyReportID { get; set; }

        public HourlyReport HourlyReports { get; set; }
    }
}
