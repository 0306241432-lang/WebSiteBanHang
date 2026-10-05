namespace Duanbanhang.ViewModels
{
    public class CartLineViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => Price * Quantity;
    }

    public class CartViewModel
    {
        public List<CartLineViewModel> Lines { get; set; } = new List<CartLineViewModel>();
        public decimal Total => Lines.Sum(l => l.LineTotal);
        public int ItemCount => Lines.Sum(l => l.Quantity);
        public bool IsEmpty => Lines.Count == 0;
    }

    /// <summary>Dữ liệu một dòng khi bấm "Cập nhật giỏ hàng".</summary>
    public class CartUpdateItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
