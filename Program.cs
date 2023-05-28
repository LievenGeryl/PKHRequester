using System;
using System.IO;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Program program = new();
        for (int i = 1; i < 11; i++)
        {
            Console.WriteLine($"Sending request {i}...");
            await program.MakeRequest();
            await Task.Delay(500);
        }
    }

    public async Task MakeRequest()
    {
        HttpClient client = new();
        var uri = "http://data.pkhonor.net/data/raw/item_prices.txt";
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, "PKHRequesterOutput.txt");
        string stackPath = Path.Combine(desktopPath, "PKHRequesterStackTrace.txt");
        try
        {
            var response = await client.GetAsync(uri);
            Console.WriteLine(response.StatusCode + " \n Writing content to PKHRequesterOutput.txt...");
            var readableContent = await response.Content.ReadAsStringAsync();
            string content = $"{DateTime.Now} \n {readableContent} \n ------------------ \n";

            try
            {
                // Write the content to the file
                await File.AppendAllTextAsync(filePath, content);
                Console.WriteLine("Content written to PKHRequesterOutput.txt successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message + "\n Stack trace will be written to file PKHRequesterStackTrace.txt");
            try
            {
                await File.AppendAllTextAsync(stackPath, "|||| START OF STACK TRACE |||| \n \n \n");
                for (int i = 0; i < ex.StackTrace.Length; i++)
                {
                    await File.AppendAllTextAsync(stackPath, ex.StackTrace);
                }
                await File.AppendAllTextAsync(stackPath, "|||| END OF STACK TRACE |||| \n \n \n");

                Console.WriteLine("Content written to PKHRequesterStackTrace.txt successfully.");
            }
            catch (Exception ex2)
            {
                Console.WriteLine("An error occurred while writing the stack trace: " + ex2.Message);
            }
        }
    }
}