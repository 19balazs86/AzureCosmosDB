using Microsoft.Azure.Cosmos;

namespace AzureCosmosDB.Types;

public interface IIdentifiable
{
    public static abstract string PartitionKeyPath { get; }
    public static abstract string ContainerName    { get; }

    Guid Id { get; }

    PartitionKey PartitionKey { get; }
}
