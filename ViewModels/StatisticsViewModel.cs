namespace Duanbanhang.ViewModels
{
    public class MonthRevenueItem
    {
        public string Label { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class StatusCountItem
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }

    public class TopProductItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
    }

    public class StatisticsViewModel
    {
        public List<MonthRevenueItem> Months { get; set; } = new List<MonthRevenueItem>();
        public List<StatusCountItem> Statuses { get; set; } = new List<StatusCountItem>();
        public List<TopProductItem> TopProducts { get; set; } = new List<TopProductItem>();
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}