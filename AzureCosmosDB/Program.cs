using AzureCosmosDB.Model;
using AzureCosmosDB.Repository;
using AzureCosmosDB.Types;
using Microsoft.Azure.Cosmos;

namespace AzureCosmosDB;

public static class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            string connectionString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_CosmosDB")
                ?? throw new NullReferenceException("Missing CosmosDB connection string.");

            using var cosmosClient = new CosmosClient(connectionString);

            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync("PlayingWithCosmosDB");

            // Put a break point here and F10
            // --> Delete: Container
            Container container = await database.deleteAndCreateContainerAsync();
            //Container container = database.GetContainer(_containerName);

            var repository = new StudentRepository(container);

            // --> Seed.
            await insertStudents(repository);

            Student student = Student.GenerateStudent(gradeLevel: 1);

            ItemResponse<Student> upsertItemResult = await repository.UpsertItemAsync(student);
            // upsertItemResult.RequestCharge

            int missingDays = await repository.IncrementMissingDays(student.Id, student.GradeLevel);

            student = await repository.ReadItemAsync(student.Id, student.PartitionKey);

            ItemResponse<Student> deleteResult = await repository.DeleteItemAsync(student);

            bool exists = await repository.ExistsAsync(student);

            int count = await repository.CountAsync(s => s.Subjects.Contains("Physics"));

            // --> Paged query
            PageQuery<Student> pageQuery = PageQuery<Student>
                .Create(pageSize: 20, partitionKey: null)
                .Filter(s => s.GradeLevel >= 1 && s.GradeLevel <= 3)
                .Sort(s => s.GradeLevel);

            PageResult<Student> studentPageResult;

            do
            {
                studentPageResult = await repository.BrowseAsync(pageQuery);

                pageQuery.ContinuationToken = studentPageResult.ContinuationToken;
            } while (studentPageResult.HasMoreItems);

            // --> Paged query with projection
            PageQuery<Student, StudentDto> pageQueryProjection = PageQuery<Student, StudentDto>
                .Create(pageSize: 20, partitionKey: null)
                .Filter(s => s.GradeLevel >= 1 && s.GradeLevel <= 3)
                .Project(s => new StudentDto { Id = s.Id, GradeLevel = s.GradeLevel, Name = s.Name, SubjectCount = s.Subjects.Count() })
                .Sort(s => s.GradeLevel);

            PageResult<StudentDto> studentDtoPageResult;

            do
            {
                studentDtoPageResult = await repository.BrowseAsync(pageQueryProjection);

                pageQueryProjection.ContinuationToken = studentDtoPageResult.ContinuationToken;
            } while (studentDtoPageResult.HasMoreItems);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static async Task<Container> deleteAndCreateContainerAsync(this Database database)
    {
        try
        {
            Container container = database.GetContainer(Student.ContainerName);

            await container.DeleteContainerAsync();
        }
        catch
        {
            // GetContainer throws exception if it does not exists
        }

        return await database.CreateContainerAsync(Student.ContainerName, partitionKeyPath: Student.PartitionKeyPath);
        //return await database.CreateContainerIfNotExistsAsync(Student.ContainerName, partitionKeyPath: Student.PartitionKeyPath);
    }

    private static async Task insertStudents(IStudentRepository repository)
    {
        for (int gradeLevel = 1; gradeLevel <= 12; gradeLevel++)
        {
            IEnumerable<Student> students = Student.GenerateStudents(gradeLevel, 10);

            await repository.CreateItemsAsync(students, new PartitionKey(gradeLevel));
        }
    }
}
