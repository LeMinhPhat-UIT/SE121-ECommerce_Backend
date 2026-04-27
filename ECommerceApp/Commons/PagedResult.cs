namespace ECommerceApp.Commons
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public PaginationRequest PaginationRequest { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => PaginationRequest == null || PaginationRequest.PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PaginationRequest.PageSize);

        public bool HasNextPage => PaginationRequest != null && PaginationRequest.PageIndex < TotalPages;
        public bool HasPreviousPage => PaginationRequest != null && PaginationRequest.PageIndex > 1;

        public PagedResult()
        {
            Items = new List<T>();
            PaginationRequest = new PaginationRequest();
        }

        public PagedResult(IEnumerable<T> itemList, PaginationRequest paginationRequest, int totalCount)
        {
            Items = itemList;
            PaginationRequest = paginationRequest;
            TotalCount = totalCount;
        }
    }
}
