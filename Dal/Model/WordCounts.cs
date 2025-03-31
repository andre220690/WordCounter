using LinqToDB.Mapping;

namespace WordCounter.Dal.Model;

[Table(Schema = "dbo", Name = "WordCounts")]
public class WordCounts
{
    [Column("word"), NotNull]
    public string Word { get; set; }

    [Column("count"), NotNull]
    public int Count { get; set; }
}
