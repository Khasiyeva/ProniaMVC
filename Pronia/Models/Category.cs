using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Category
    {
        public int Id { get; set; }
        [StringLength(10)]
        public string Name { get; set; }
        public List<Product>? Products { get; set;}
    }
}
