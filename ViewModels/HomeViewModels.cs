using Duanbanhang.Helpers;
using Duanbanhang.Models;

namespace Duanbanhang.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> NewProducts { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public Product Featured { get; set; }
    }

    public class ShopViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public IPagedList Paging { get; set; }
        public List<Product> Recommended { get; set; } = new List<Product>();
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Query { get; set; }
    }

    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public List<Product> Related { get; set; } = new List<Product>();
    }
}