namespace ECommerceApp.Commons
{
    public static class PaginationExtensions
    {
        public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> items, PaginationRequest paginationRequest)
        {
            var materializedItems = items.ToList();
            var safeRequest = new PaginationRequest
            {
                PageIndex = paginationRequest?.PageIndex > 0 ? paginationRequest.PageIndex : 1,
                PageSize = paginationRequest?.PageSize > 0 ? paginationRequest.PageSize : 10
            };

            var totalCount = materializedItems.Count;
            var pagedItems = materializedItems
                .Skip((safeRequest.PageIndex - 1) * safeRequest.PageSize)
                .Take(safeRequest.PageSize)
                .ToList();

            return new PagedResult<T>(pagedItems, safeRequest, totalCount);
        }
    }
}