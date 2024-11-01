using AzureCosmosDB.Types;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace AzureCosmosDB.Cosmos;

public static class Pagination
{
    public static async Task<PageResult<T>> PaginateAsync<T>(this Container container, PageQuery<T> query)
    {
        IQueryable<T> queryableItems = container.initQueryable(query);

        return await queryableItems.paginateAsync();
    }

    public static async Task<PageResult<P>> PaginateAsync<T, P>(this Container container, PageQuery<T, P> query)
    {
        IQueryable<P> queryableItems = container
            .initQueryable(query)
            .Select(query.ProjectionDefinition);

        return await queryableItems.paginateAsync();
    }

    private static IQueryable<T> initQueryable<T>(this Container container, PageQuery<T> query)
    {
        if (query.FilterDefinition is null)
        {
            throw new NullReferenceException("PageQuery.FilterDefinition can not be null.");
        }

        var requestOptions = new QueryRequestOptions
        {
            PartitionKey = query.PartitionKey,
            MaxItemCount = query.PageSize
        };

        IQueryable<T> queryableItems = container
            .GetItemLinqQueryable<T>(continuationToken: query.ContinuationToken, requestOptions: requestOptions)
            .Where(query.FilterDefinition);

        if (query.SortDefinition is not null)
        {
            queryableItems = query.SortDefinition(queryableItems);
        }

        return queryableItems;
    }

    private static async Task<PageResult<P>> paginateAsync<P>(this IQueryable<P> queryableItems)
    {
        using FeedIterator<P> feedIterator = queryableItems.ToFeedIterator();

        if (!feedIterator.HasMoreResults)
        {
            return PageResult<P>.Empty;
        }

        FeedResponse<P> feedResponse = await feedIterator.ReadNextAsync();

        return new PageResult<P>(feedResponse.Resource, feedResponse.ContinuationToken);
    }
}
