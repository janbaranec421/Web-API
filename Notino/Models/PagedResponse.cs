namespace Notino.Models
{
    public record PagedResponse<T>
    {
        public int PageIndex { get; init; }
        public int PageSize { get; init; }
        public int TotalRecords { get; init; }
        public int TotalPages { get; init; }
        public List<T> Data { get; init; }

        public PagedResponse(List<T> data, int pageIndex, int pageSize, int totalRecords)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)pageSize);

        }
    }
}
