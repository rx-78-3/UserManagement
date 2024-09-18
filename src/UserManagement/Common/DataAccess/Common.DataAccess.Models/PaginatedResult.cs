namespace Common.DataAccess.Models;

public record PaginatedResult<TEntity>(long TotalCount, IEnumerable<TEntity> Data)
    where TEntity : class;
