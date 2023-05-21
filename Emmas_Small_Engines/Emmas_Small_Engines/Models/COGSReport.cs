using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Emmas_Small_Engines.Models
{
    public class COGSReport
    {
        public int ID { get; set; }

        [Display(Name = "Report Name")]
        public string cogName { get; set; }

        [Display(Name = "Date Start")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Date End")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name = "COGS Report Criteria")]
        [Required(ErrorMessage = "The COGS Report Criteria cannot be empty.")]
        [StringLength(100, ErrorMessage = "The COGS Report Criteria cannot be more than 100 characters long.")]
        public string crepCriteria { get; set; }

        [Display(Name = "Materials (Period Start)")]
        [DataType(DataType.Currency)]
        public double MaterialPeriodStart { get; set; }

        [Display(Name = "Materials (Period Purchase)")]
        [DataType(DataType.Currency)]
        public double MaterialPeriodPurchase { get; set; }

        [Display(Name = "Materials (Period End)")]
        [DataType(DataType.Currency)]
        public double MaterialPeriodEnd { get; set; }

        [Display(Name = "Costs of Goods Sold")]
        [DataType(DataType.Currency)]
        public double COGS { get; set; }

        [Display(Name = "Sale Revenue")]
        [DataType(DataType.Currency)]
        public double SaleRevenuePeriod { get; set; }

        [Display(Name = "Gross Profit")]
        [DataType(DataType.Currency)]
        public double GrossProfit { get; set; }

        [Display(Name = "Profit Margin")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double ProfitMargin { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.Now;


        [Timestamp]
        public Byte[] RowVersion { get; set; }

        //UPC
        public string InventoryID { get; set; }

        public Inventory Inventory { get; set; }

        //Invoice
        public int InvoiceID { get; set; }

        public Invoice Invoice { get; set; }

        
        


    }
}
