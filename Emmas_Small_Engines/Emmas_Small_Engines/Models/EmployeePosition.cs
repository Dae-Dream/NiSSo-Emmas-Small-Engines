namespace Emmas_Small_Engines.Models
{
    public class EmployeePosition
    {
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        //not sure about this
        public string Title { get; set; }
        public Position Position { get; set; }
    }
}
