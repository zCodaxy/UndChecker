using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main()
    {
        string undlogo = @"
 _   _ _  _ ___   ___ _  _ ___ ___ _  _____ ___ 
| | | | \| |   \ / __| || | __/ __| |/ / __| _ \
| |_| | .` | |) | (__| __ | _| (__| ' <| _||   /
 \___/|_|\_|___/ \___|_||_|___\___|_|\_\___|_|_\ 
";

        string apiKey = "put key here";

        string inputFile = "input.txt";
        string[] lines = File.ReadAllLines(inputFile);

        string outputFile = "undeliverable.txt";
        StreamWriter writer = new StreamWriter(outputFile);

        using (HttpClient client = new HttpClient())
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{undlogo}\nVersion 1.0.1 | Made By Codaxy\n");
            Console.ResetColor();

            Console.CursorVisible = false;
            int totalLines = lines.Length;
            int currentLine = 0;

            foreach (string line in lines)
            {
                int firstColonIndex = line.IndexOf(':');

                if (firstColonIndex >= 0)
                {
                    string email = line.Substring(0, firstColonIndex).Trim();
                    string resto = line.Substring(firstColonIndex + 1).Trim();

                    string apiUrl = "https://api.kickbox.com/v2/verify";
                    string url = $"{apiUrl}?email={email}&apikey={apiKey}";

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        JObject json = JObject.Parse(responseBody);

                        string result = json["result"].ToString();
                        if (result == "undeliverable")
                        {
                            writer.WriteLine($"Undeliverable - {email}:{resto}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{email}: {response.StatusCode}");
                    }
                }

                currentLine++;
                float progress = (float)currentLine / totalLines;
                UpdateProgressBar(progress);
            }
        }

        writer.Close();

        Console.CursorVisible = true;
        Console.WriteLine("\n\nDone. Press the key Space to close.");
        while (Console.ReadKey().Key != ConsoleKey.Spacebar) { }
    }

    static void UpdateProgressBar(float progress)
    {
        int width = Console.WindowWidth - 2;
        int position = (int)(width * progress);

        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write("[");

        for (int i = 0; i < width; i++)
        {
            if (i < position)
            {
                Console.Write("=");
            }
            else
            {
                Console.Write(" ");
            }
        }

        Console.Write("]");
    }
}
