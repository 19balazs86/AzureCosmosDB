using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;

namespace AzureCosmosDB.Types;

public static class PageQueryDefaults
{
    public const int PageSizeDefault = 20;
    public const int PageSizeMax     = 50;
}

public class PageQuery<TEntity>
{
    private int _pageSize;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is <= 0 or > PageQueryDefaults.PageSizeMax ? PageQueryDefaults.PageSizeDefault : value;
    }

    public string? ContinuationToken { get; set; }

    public PartitionKey? PartitionKey { get; set; }

    public Expression<Func<TEntity, bool>> FilterDefinition { get; set; }

    public Func<IQueryable<TEntity>, IQueryable<TEntity>>? SortDefinition { get; set; }

    public PageQuery(int pageSize = PageQueryDefaults.PageSizeDefault)
    {
        PageSize = pageSize;

        // Default values
        FilterDefinition = _ => true;
    }

    public static PageQuery<TEntity> Create(int pageSize = PageQueryDefaults.PageSizeDefault, PartitionKey? partitionKey = null)
    {
        return new PageQuery<TEntity>(pageSize) { PartitionKey = partitionKey };
    }
}

public class PageQuery<TEntity, TProjection> : PageQuery<TEntity>
{
    public Expression<Func<TEntity, TProjection>> ProjectionDefinition { get; set; }

    public PageQuery(int pageSize = PageQueryDefaults.PageSizeDefault) : base(pageSize)
    {
        ProjectionDefinition = _ => default!;
    }

    public new static PageQuery<TEntity, TProjection> Create(int pageSize = PageQueryDefaults.PageSizeDefault, PartitionKey? partitionKey = null)
    {
        return new PageQuery<TEntity, TProjection>(pageSize) { PartitionKey = partitionKey };
    }
}