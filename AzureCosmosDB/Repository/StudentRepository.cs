using AzureCosmosDB.Cosmos;
using AzureCosmosDB.Model;
using Microsoft.Azure.Cosmos;

namespace AzureCosmosDB.Repository;

public sealed class StudentRepository : CosmosRepository<Student>, IStudentRepository
{
    public StudentRepository(Container container) : base(container)
    {
    }

    public async Task<int> IncrementMissingDays(Guid id, int gradeLevel)
    {
        string path = $"/{nameof(Student.NumberOfMissingDays)}";

        var operations = new PatchOperation[]
        {
            PatchOperation.Increment(path, 1)
        };

        Student student = await _container.PatchItemAsync<Student>(id.ToString(), new PartitionKey(gradeLevel), operations);

        return student.NumberOfMissingDays;
    }
}
