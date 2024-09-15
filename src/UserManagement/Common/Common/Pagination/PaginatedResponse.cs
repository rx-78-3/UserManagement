using Common.DataAccess.Models;

namespace Common.Pagination;

public record PaginatedResponse<TEntity>()
    where TEntity : class
{
    public PaginatedResponse(PaginationRequest request, PaginatedResult<TEntity> result) : this()
    {
        PageIndex = request.PageIndex;
        PageSize = request.PageSize;
        Count = result.Count;
        Data = result.Data;
    }

    public int PageIndex { get; }
    public int PageSize { get; }
    public long Count { get; }
    public IEnumerable<TEntity> Data { get; } = Array.Empty<TEntity>();
}
