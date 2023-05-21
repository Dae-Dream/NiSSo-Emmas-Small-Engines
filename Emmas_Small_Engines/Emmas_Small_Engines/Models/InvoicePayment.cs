/*Written by Jia Ni Zhao
 On Feb 17, 2022*/
namespace Emmas_Small_Engines.Models
{
    public class InvoicePayment
    {
        public int InvoiceID { get; set; }
        public Invoice Invoice { get; set; }

        public int PaymentID { get; set; }

        public Payment Payment { get; set; }
    }
}
