using Common.DataAccess.Models;

namespace Common.Pagination;

public record PaginatedResponse<TEntity>()
    where TEntity : class
{
    public PaginatedResponse(PaginationRequest request, PaginatedResult<TEntity> result) : this()
    {
        PageIndex = request.PageIndex;
        PageSize = request.PageSize;
        TotalCount = result.TotalCount;
        Data = result.Data;
    }

    public int PageIndex { get; }
    public int PageSize { get; }
    public long TotalCount { get; }
    public IEnumerable<TEntity> Data { get; } = Array.Empty<TEntity>();
}
