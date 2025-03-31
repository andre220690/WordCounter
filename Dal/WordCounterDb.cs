using LinqToDB;
using LinqToDB.Data;
using WordCounter.Dal.Model;

namespace WordCounter.Dal;

public class WordCounterDb : DataConnection
{
    public WordCounterDb(string connectionString) : base("SqlServer", connectionString) { }

    public IQueryable<WordCounts> WordCounters => this.GetTable<WordCounts>();
}
