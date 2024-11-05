namespace Notino.Dtos
{
    public record PagedResponseDto<T>
    {
        public int PageIndex { get; init; }
        public int PageSize { get; init; }
        public int TotalRecords { get; init; }
        public int TotalPages { get; init; }
        public List<T> Data { get; init; }
    }
}
