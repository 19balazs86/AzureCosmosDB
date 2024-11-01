using AzureCosmosDB.Types;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AzureCosmosDB.Model;

public enum PreferredLanguage
{
    Chinese = 0, Spanish, English, Arabic, Hindi, Bengali, Portuguese, Russian, Japanese, Lahnda
}

public sealed class Student : IIdentifiable
{
    public static string PartitionKeyPath => "/GradeLevel";
    public static string ContainerName    => nameof(Student);

    [JsonIgnore]
    public PartitionKey PartitionKey => new PartitionKey(GradeLevel);

    private static readonly DateTime _now      = DateTime.Now;
    private static readonly DateTime _toDate   = _now.AddYears(-10);
    private static readonly DateTime _fromDate = _toDate.AddYears(-90);

    private static readonly Random _random     = new Random();
    private static readonly string[] _subjects = ["English", "Mathematics", "Physics", "Chemistry", "Spanish"];

    [JsonProperty("id")]
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public int GradeLevel { get; set; } // 1-12

    [JsonConverter(typeof(StringEnumConverter))]
    public PreferredLanguage PreferredLanguage { get; set; }

    public int NumberOfMissingDays { get; set; }

    public ICollection<string> Subjects { get; set; } = []; // IEnumerable is not working

    public static Student GenerateStudent(int gradeLevel, int? i = null)
    {
        if (gradeLevel is < 1 or > 12)
        {
            throw new IndexOutOfRangeException("GradeLevel must be 1 - 12");
        }

        DateTime dateOfBirth = getRandomDate(_fromDate, _toDate);

        return new Student
        {
            Id                  = Guid.NewGuid(),
            Name                = $"GL{gradeLevel} - Name #{i ?? _random.Next(100, 1000)}",
            DateOfBirth         = DateOnly.FromDateTime(dateOfBirth.Date),
            GradeLevel          = gradeLevel,
            Subjects            = shuffleSubjects(),
            PreferredLanguage   = (PreferredLanguage)_random.Next(0, Enum.GetValues<PreferredLanguage>().Length),
            NumberOfMissingDays = _random.Next(0, 15)
        };
    }

    public static IEnumerable<Student> GenerateStudents(int gradeLevel, ushort count)
    {
        return Enumerable.Range(1, count).Select(i => GenerateStudent(gradeLevel, i)).ToList();
    }

    private static string[] shuffleSubjects()
    {
        int length = _random.Next(_subjects.Length);

        return _random.GetItems(_subjects, length);
    }

    //private static ICollection<string> shuffleSubjects_BeforeNet8()
    //{
    //    var list = new List<string>(_subjects);

    //    for (int n = list.Count - 1; n > 0; n--)
    //    {
    //        int swapIndex = _random.Next(n + 1);

    //        (list[n], list[swapIndex]) = (list[swapIndex], list[n]);
    //    }

    //    return list.Take(_random.Next(_subjects.Length)).ToList();
    //}

    private static DateTime getRandomDate(DateTime from, DateTime to)
    {
        var range = new TimeSpan(to.Ticks - from.Ticks);

        return from + new TimeSpan((long)(range.Ticks * _random.NextDouble()));
    }
}
