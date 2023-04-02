using Microsoft.Azure.Cosmos;

namespace AzureCosmosDB.Types;

public interface IIdentifiable
{
    Guid Id { get; }

    PartitionKey PartitionKey { get; }
}