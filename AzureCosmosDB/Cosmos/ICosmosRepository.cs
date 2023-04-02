using AzureCosmosDB.Types;
using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;

namespace AzureCosmosDB.Cosmos;

public interface ICosmosRepository<TEntity> where TEntity : IIdentifiable
{
    Task<ItemResponse<TEntity>> ReadItemAsync(Guid id, PartitionKey partitionKey);

    IAsyncEnumerable<TEntity> FindAsync(PartitionKey partitionKey);

    IAsyncEnumerable<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, PartitionKey? partitionKey = null);

    Task<PageResult<TEntity>> BrowseAsync(PageQuery<TEntity> pageQuery);

    Task<PageResult<TProjection>> BrowseAsync<TProjection>(PageQuery<TEntity, TProjection> pageQuery);

    Task<ItemResponse<TEntity>> CreateItemAsync(TEntity entity);

    Task<bool> CreateItemsAsync(IEnumerable<TEntity> entities, PartitionKey partitionKey);

    Task<ItemResponse<TEntity>> UpsertItemAsync(TEntity entity);

    Task<ItemResponse<TEntity>> DeleteItemAsync(TEntity entity);

    Task<bool> ExistsAsync(TEntity entity);

    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, PartitionKey? partitionKey = null);
}