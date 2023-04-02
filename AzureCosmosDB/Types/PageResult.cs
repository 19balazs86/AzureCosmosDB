using System.Diagnostics;

namespace AzureCosmosDB.Types;

[DebuggerDisplay("HasMoreItems = {HasMoreItems}")]
public sealed class PageResult<TEntity>
{
    public IEnumerable<TEntity> Items { get; private set; }

    public string? ContinuationToken { get; private set; }

    public bool HasMoreItems => !string.IsNullOrWhiteSpace(ContinuationToken);

    public static PageResult<TEntity> Empty => new PageResult<TEntity>();

    public PageResult()
    {
        Items = Enumerable.Empty<TEntity>();
    }

    public PageResult(IEnumerable<TEntity> items, string? continuationToken)
    {
        Items = items;

        ContinuationToken = continuationToken;
    }
}