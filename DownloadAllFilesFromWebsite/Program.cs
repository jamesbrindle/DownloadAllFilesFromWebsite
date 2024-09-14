using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

public class Program
{
    public static async Task Main(string[] args)
    {
        string websiteUrl = "https://ironsoftware.com/csharp/ocr/languages/"; // Replace with the actual URL
        string downloadFolder = @"E:\Downloads"; // Specify the folder where you want to save the zip files

        // Ensure download directory exists
        if (!Directory.Exists(downloadFolder))
        {
            Directory.CreateDirectory(downloadFolder);
        }

        await DownloadZipFilesFromWebsite(websiteUrl, downloadFolder);
    }

    public static async Task DownloadZipFilesFromWebsite(string url, string downloadFolder)
    {
        HttpClient httpClient = new HttpClient();

        try
        {
            // Download the HTML content of the page
            string htmlContent = await httpClient.GetStringAsync(url);

            // Load the HTML content into HtmlAgilityPack
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Find all the hyperlinks (anchor tags) that end with .zip
            var zipLinks = htmlDoc.DocumentNode.SelectNodes("//a[@href]")
                .Select(node => node.GetAttributeValue("href", null))
                .Where(href => href != null && href.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));

            foreach (var zipLink in zipLinks)
            {
                // If the link is relative, make it absolute
                string absoluteUrl = zipLink.StartsWith("http") ? zipLink : new Uri(new Uri(url), zipLink).ToString();

                Console.WriteLine($"Downloading: {absoluteUrl}");

                // Download the zip file
                await DownloadFileAsync(absoluteUrl, downloadFolder);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static async Task DownloadFileAsync(string fileUrl, string downloadFolder)
    {
        HttpClient httpClient = new HttpClient();

        try
        {
            // Get the file name from the URL
            string fileName = Path.GetFileName(new Uri(fileUrl).AbsolutePath);

            // Specify the local path where the file will be saved
            string filePath = Path.Combine(downloadFolder, fileName);

            // Download the file
            byte[] fileBytes = await httpClient.GetByteArrayAsync(fileUrl);

            // Save the file to the local folder
            File.WriteAllBytes(filePath, fileBytes);

            Console.WriteLine($"Downloaded and saved: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to download {fileUrl}. Error: {ex.Message}");
        }
    }
}
