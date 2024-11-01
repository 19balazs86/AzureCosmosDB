using System.Linq.Expressions;
using AzureCosmosDB.Types;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace AzureCosmosDB.Cosmos;

public class CosmosRepository<TEntity>(Container _container) : ICosmosRepository<TEntity> where TEntity : IIdentifiable
{
    protected readonly Container _container = _container;

    public virtual async Task<ItemResponse<TEntity>> ReadItemAsync(Guid id, PartitionKey partitionKey)
    {
        return await _container.ReadItemAsync<TEntity>(id.ToString(), partitionKey);
    }

    public virtual async IAsyncEnumerable<TEntity> FindAsync(PartitionKey partitionKey)
    {
        var requestOptions = new QueryRequestOptions { PartitionKey = partitionKey };

        using FeedIterator<TEntity> feedIterator = _container.GetItemQueryIterator<TEntity>(requestOptions: requestOptions);

        while (feedIterator.HasMoreResults)
        {
            foreach (TEntity entity in await feedIterator.ReadNextAsync())
            {
                yield return entity;
            }
        }
    }

    public virtual async IAsyncEnumerable<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, PartitionKey? partitionKey = null)
    {
        var requestOptions = new QueryRequestOptions { PartitionKey = partitionKey };

        using FeedIterator<TEntity> feedIterator = _container
            .GetItemLinqQueryable<TEntity>(requestOptions: requestOptions)
            .Where(predicate)
            .ToFeedIterator();

        while (feedIterator.HasMoreResults)
        {
            foreach (TEntity entity in await feedIterator.ReadNextAsync())
            {
                yield return entity;
            }

            //FeedResponse<TEntity> feedResponse = await feedIterator.ReadNextAsync();

            //Console.WriteLine("RequestCharge: {0}", feedResponse.RequestCharge);

            //foreach (TEntity entity in feedResponse.Resource)
            //    yield return entity;
        }
    }

    public virtual async Task<PageResult<TEntity>> BrowseAsync(PageQuery<TEntity> pageQuery)
    {
        return await _container.PaginateAsync(pageQuery);
    }

    public virtual async Task<PageResult<TProjection>> BrowseAsync<TProjection>(PageQuery<TEntity, TProjection> pageQuery)
    {
        return await _container.PaginateAsync(pageQuery);
    }

    public virtual async Task<ItemResponse<TEntity>> CreateItemAsync(TEntity entity)
    {
        return await _container.CreateItemAsync(entity);
    }

    public virtual async Task<bool> CreateItemsAsync(IEnumerable<TEntity> entities, PartitionKey partitionKey)
    {
        TransactionalBatch batch = _container.CreateTransactionalBatch(partitionKey);

        foreach (TEntity entity in entities)
        {
            batch.CreateItem(entity);
        }

        using TransactionalBatchResponse batchResponse = await batch.ExecuteAsync();

        return batchResponse.IsSuccessStatusCode;
    }

    public virtual async Task<ItemResponse<TEntity>> UpsertItemAsync(TEntity entity)
    {
        return await _container.UpsertItemAsync(entity);
    }

    public virtual async Task<ItemResponse<TEntity>> DeleteItemAsync(TEntity entity)
    {
        return await _container.DeleteItemAsync<TEntity>(entity.Id.ToString(), entity.PartitionKey);
    }

    public virtual async Task<bool> ExistsAsync(TEntity entity)
    {
        using ResponseMessage response = await _container.ReadItemStreamAsync(entity.Id.ToString(), entity.PartitionKey);

        return response.IsSuccessStatusCode;
    }

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, PartitionKey? partitionKey = null)
    {
        var queryRequestOptions = new QueryRequestOptions { PartitionKey = partitionKey };

        return await _container
            .GetItemLinqQueryable<TEntity>(requestOptions: queryRequestOptions)
            .Where(predicate)
            .CountAsync();
    }
}
