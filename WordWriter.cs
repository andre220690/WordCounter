using LinqToDB;
using Microsoft.Extensions.Configuration;
using System.Data;
using WordCounter.Dal;
using WordCounter.Dal.Model;

namespace WordCounter;

public class WordWriter
{
    private const int PACKEGE_SIZE = 1000;

    public void ClearDB()
    {
        string connectionString = GetConnectionString();

        Console.WriteLine("Начало очистки таблицы в БД.");

        using var db = new WordCounterDb(connectionString);
        using var transaction = db.BeginTransaction(IsolationLevel.Serializable);

        db.WordCounters.Delete();

        transaction.Commit();

        Console.WriteLine("Таблица в БД очищена");
    }

    // Записываем в базу через транзакцию repeatableRead, т.к. возможна парралельная запись из другого приложения
    // Для того что бы не блокировть параллельную запись, делим массив слов на пакеты
    // Есть проблема, что нет атомарности. Может записаться только половина пакетов.
    // ЧТо бы этого избежать, можно отказаться от пакетов и использовать уровень изоляции Serializable
    public void WriteToDB(IEnumerable<WordCounts> words)
    {
        string connectionString = GetConnectionString();

        Console.WriteLine("Начало записи в БД.");

        using var db = new WordCounterDb(connectionString);

        var packegeNumber = 1;
        foreach (var packege in words.Chunk(PACKEGE_SIZE))
        {
            using var transaction = db.BeginTransaction(IsolationLevel.RepeatableRead);
            {
                db.WordCounters
                    .Merge()
                    .Using(packege)
                    .On((t, s) => t.Word == s.Word)
                    .UpdateWhenMatched((t, s) => new WordCounts { Count = t.Count + s.Count })
                    .InsertWhenNotMatched(s => s)
                    .Merge();

                transaction.Commit();
            }

            Console.WriteLine($"Отправлен пакет номер {packegeNumber++} из {packege.Length} слов.");
        }

        Console.WriteLine("Завершение записи в БД.");
    }

    private string GetConnectionString()
    {
        var configuration = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();

        string connectionString = configuration.GetConnectionString("WordCounterDb");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Отсутствует ссылка для подключения к БД");
        }

        return connectionString;
    }
}
