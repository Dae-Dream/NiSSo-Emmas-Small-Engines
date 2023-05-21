using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Emmas_Small_Engines.Models
{
    public class ReportData
    {
        public int ID { get; set; }

        [Display(Name = "Report Name")]
        public string repName { get; set; }

        [Display(Name = "Date Start")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateStart { get; set; }

        [Display(Name = "Date End")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateEnd { get; set; }

        [Display(Name = "Criteria")]
        public string repCriteria { get; set; }

        [Display(Name = "Type")]
        public string repType { get; set; }
    }
}
