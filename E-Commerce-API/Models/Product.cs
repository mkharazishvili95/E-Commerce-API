using System.Xml.Linq;

namespace E_Commerce_API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string ProductCategory { get; set; }
        public string InStock { get; set; }
        public double Price { get; set; }
    }
}
