using LinqToDB;
using System.Data;
using WordCounter;
using WordCounter.Dal.Model;

// TODO: добавить комментарии к транзакции

Console.WriteLine("Введите номер требуемой операции:");
Console.WriteLine("1. Чтение файла");
Console.WriteLine("2. Очистка базы данных");

try
{
    var planNumber = Console.ReadLine();

    switch (planNumber)
    {
        case "1":
            ExecuteFileProcessing();
            break;

        case "2":
            ExecuteCleaning();
            break;

        default:
            Console.WriteLine("Не верно указан номер операции");
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine("Завершение программы");


void ExecuteCleaning()
{
    var wordWriter = new WordWriter();
    wordWriter.ClearDB();
}

void ExecuteFileProcessing()
{
    Console.WriteLine("Введите путь к файлу");
    var path = Console.ReadLine();

    var fileReader = new FileReader();
    var wordCounterDictionary = fileReader.GetWordsFromFile(path);

    var words = wordCounterDictionary.Where(r => r.Value >= 2).Select(r => new WordCounts() { Word = r.Key, Count = r.Value }).ToArray();

    var wordWriter = new WordWriter();
    wordWriter.WriteToDB(words);
}