using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace WordCounter;

public class FileReader
{
    public IDictionary<string, int> GetWordsFromFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Файл не существует");
        }

        var wordCounterDictionary = new ConcurrentDictionary<string, int>();

        Console.WriteLine("Начало чтения файла");

        var lines = File.ReadAllLines(path, Encoding.UTF8);
        Parallel.ForEach(lines, line =>
        {
            var words = Regex.Split(line, @"\P{L}+");

            foreach (var word in words)
            {
                if (word.Length < 4 || word.Length > 20)
                    continue;

                var normalizedWord = word.ToLower().Replace('ё', 'е');

                wordCounterDictionary.AddOrUpdate(normalizedWord, 1, (word, count) => count + 1);
            }
        });

        Console.WriteLine("Завершение чтения файла");
        return wordCounterDictionary;
    }
}
