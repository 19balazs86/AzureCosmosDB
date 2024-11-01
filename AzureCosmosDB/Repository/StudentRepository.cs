using AzureCosmosDB.Cosmos;
using AzureCosmosDB.Model;
using Microsoft.Azure.Cosmos;

namespace AzureCosmosDB.Repository;

public sealed class StudentRepository(Container _container) : CosmosRepository<Student>(_container), IStudentRepository
{
    public async Task<int> IncrementMissingDays(Guid id, int gradeLevel)
    {
        const string path = $"/{nameof(Student.NumberOfMissingDays)}";

        PatchOperation[] operations =
        [
            PatchOperation.Increment(path, 1)
        ];

        Student student = await _container.PatchItemAsync<Student>(id.ToString(), new PartitionKey(gradeLevel), operations);

        return student.NumberOfMissingDays;
    }
}
