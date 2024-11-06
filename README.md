# Playing with Azure CosmosDB

- This is a .NET console application designed as a playground for working with Azure CosmosDB (NoSQL)

## Resources

#### 📚 `Microsoft-Learn`

- [Quickstart](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/quickstart-dotnet)
- [Get started](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/how-to-dotnet-get-started) *- Working with Database, Container and Items*
- [Partial document update with Patch Operations](https://learn.microsoft.com/en-us/azure/cosmos-db/partial-document-update-getting-started)
- [Azure SDK - Container Class](https://learn.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.container)
- [Emulator](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator)

#### ✨ `Miscellaneous`

- Pagination
  - [Tips and Tricks for query pagination](https://devblogs.microsoft.com/cosmosdb/tips-tricks-query-pagination) 📚*MS DevBlogs*
  - [Examples](https://github.com/Azure/azure-cosmos-dotnet-v3/blob/master/Microsoft.Azure.Cosmos.Samples/Usage/Queries/Program.cs) 👤*Azure - Examples for pagination and other features as well*
- [Data Explorer](https://cosmos.azure.com) 📚*Web interface*
- [Triggering an Azure Function from the CosmosDB Change Feed](https://youtu.be/LPx2vK50Th0) 📽️*14 min - Gui Ferreira*
- [Building planet scale apps, best practices](https://youtu.be/QbBSL2oBW1A) 📽️*1h:13m VS-Live/Justine Cocchi*

## In the example provided, you can find

- Repository with CRUD operations and examples of using it
- Pagination solution with `PageResult` and `PageQuery` object

```csharp
PageQuery<Student> pageQuery = PageQuery<Student>
    .Create(pageSize: 20, partitionKey: null)
    .Filter(s => s.GradeLevel >= 1 && s.GradeLevel <= 3)
    .Sort(s => s.GradeLevel);

PageResult<Student> students = await repository.BrowseAsync(pageQuery);
```

```csharp
PageQuery<Student, StudentDto> pageQuery = PageQuery<Student, StudentDto>
    .Create(...).Filter(...).Sort(...)
    .Project(s => new StudentDto { Id = s.Id, Name = s.Name });

PageResult<StudentDto> students = await repository.BrowseAsync(pageQuery);
```
