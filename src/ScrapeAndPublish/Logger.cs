namespace ScrapeAndPublish;

public class Logger
{
    public static void Info(string format, params object?[]? args)
    {
        Console.WriteLine(format, args);
    }
}
