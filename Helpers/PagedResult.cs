public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int PageNumber { get; set; }
    private int _pageSize;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(PageSize),
                    "PageSize must be greater than zero.");

            _pageSize = value;
        }
        
    }

    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

}