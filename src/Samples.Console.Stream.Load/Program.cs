using System.Text;

namespace Samples.Console.Stream.Load;

internal static class Program
{
    internal static async Task Main(string[]? args)
    {
        if (args is not { Length: 2 })
        {
            System.Console.WriteLine("Must include 2 args - batchSize and milliseconds to pause between iterations");

            return;
        }

        var batchSize = int.TryParse(args[0], out var b) && b > 0
                            ? b
                            : 500;

        var iterationDelay = int.TryParse(args[1], out var m) && m > 0
                                 ? m
                                 : 250;

        System.Console.WriteLine(string.Concat("Using batchSize of ", batchSize, " iteration delay of ", iterationDelay, "ms."));
        System.Console.WriteLine("Hit ctrl+C to end...");

        var http = new HttpClient();
        var jsonBodyBuilder = new StringBuilder(batchSize * 3);
        
        do
        {
            jsonBodyBuilder.Append('[');
            
            for (var x = 0; x < batchSize; x++)
            {
                if (x > 0)
                {
                    jsonBodyBuilder.AppendLine(",");
                }

                jsonBodyBuilder.Append(GetSnippetJson(string.Concat("user", x)));
            }

            jsonBodyBuilder.Append(']');
            var json = jsonBodyBuilder.ToString();
            jsonBodyBuilder.Clear();
            
            System.Console.WriteLine(string.Concat("Posting ", batchSize, " snippets total ", json.Length, " characters"));

            var response = await http.PostAsync("http://localhost:8082/snippets",
                                                new StringContent(json, Encoding.UTF8, "application/json"));
            
            await Task.Delay(iterationDelay);
        } while (true);
    }

    private static string GetSnippetJson(string userName)
    {
        var guid = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var tags = new[]
                   {
                       string.Concat("#unique", guid.ToString("N")[^6..]),
                       string.Concat("#minute", now.Minute),
                       string.Concat("#second", now.Second)
                   };

        var json = string.Concat(@"
{
    ""Body"": ""sample snippet body that would be a tweet/post/story/etc. body or other thing ", Guid.NewGuid().ToString("N"), @"."",
    ""UserId"": """, userName, @""",
    ""Tags"": [ """, string.Join("\",\"", tags), @""" ]
}");

        return json;
    }
}