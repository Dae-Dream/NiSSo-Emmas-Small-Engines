using System.ComponentModel.DataAnnotations;

namespace Emmas_Small_Engines.Models
{
    public class Position
    {
        [Key]
        public string Title { get; set; }

        public ICollection<EmployeePosition> EmployeePositions { get; set; } = new HashSet<EmployeePosition>();

    }
}
