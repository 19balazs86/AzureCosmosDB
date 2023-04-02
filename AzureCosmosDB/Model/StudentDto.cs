namespace AzureCosmosDB.Model;

public sealed class StudentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public int SubjectCount { get; set; }
}
