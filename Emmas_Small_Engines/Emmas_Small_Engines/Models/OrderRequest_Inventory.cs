/*Written by Jia Ni Zhao
 On Feb 17, 2022*/
namespace Emmas_Small_Engines.Models
{
    public class OrderRequest_Inventory
    {
        public int OrderAmount { get; set; }
        public int OrderRequestID { get; set; }

        public OrderRequest OrderRequest { get; set; }

        public string InventoryID { get; set; }

        public Inventory Inventory { get; set; }
    }
}
