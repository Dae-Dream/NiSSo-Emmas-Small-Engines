namespace Emmas_Small_Engines.Models
{
	public class CartItem
	{
		public string InventoryUPC { get; set; }
		public Inventory Inventory { get; set; }
		public double Price { get; set; }
		public int OrderQuantity { get; set; }
	}
}
