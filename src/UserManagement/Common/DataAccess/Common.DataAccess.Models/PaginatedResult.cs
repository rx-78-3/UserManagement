namespace Common.DataAccess.Models;

public record PaginatedResult<TEntity>(long Count, IEnumerable<TEntity> Data)
    where TEntity : class;
