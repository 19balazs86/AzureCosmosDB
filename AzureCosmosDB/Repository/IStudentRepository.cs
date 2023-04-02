using AzureCosmosDB.Cosmos;
using AzureCosmosDB.Model;

namespace AzureCosmosDB.Repository;

public interface IStudentRepository : ICosmosRepository<Student>
{
    Task<int> IncrementMissingDays(Guid id, int gradeLevel);
}
