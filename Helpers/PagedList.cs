using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Helpers
{
    public interface IPagedList
    {
        int Page { get; }
        int TotalPages { get; }
        int TotalItems { get; }
    }

    public class PagedList<T> : IPagedList
    {
        public List<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalItems { get; }
        public int TotalPages { get; }

        private PagedList(List<T> items, int page, int pageSize, int totalItems)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int page, int pageSize)
        {
            if (pageSize < 1) pageSize = 10;
            var total = await source.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            page = Math.Min(Math.Max(page, 1), totalPages);

            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, page, pageSize, total);
        }
    }
}