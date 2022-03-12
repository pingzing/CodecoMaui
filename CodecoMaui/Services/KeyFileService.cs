using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CodecoMaui.Services;

public class KeyFileService : IKeyFileService
{
    private const string KeyfileFolderName = "keyfiles";
    private readonly string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Codeco Maui Edition");

    public async Task<string?> AddKeyFile(Dictionary<string, string> keyFile, string fileName)
    {
        string targetFilename = Path.GetFileNameWithoutExtension(fileName); // Chop off any user-added extensions
        string targetPath = Path.Combine(BasePath, KeyfileFolderName, targetFilename);

        while (File.Exists($"{targetPath}.json"))
        {
            // Check for an existing de-duplicator (i.e. a (1) or a (2) or a (227)) at the end of the filename
            // If we find one, increment it by one and try again.
            Match? existingDedupe = Regex.Match(targetPath, @"\((\d{1})\)$");
            if (existingDedupe?.Success == true)
            {
                long existingNumber = long.Parse(existingDedupe.Groups[1].Value);
                targetPath = $"{targetPath.Substring(0, existingDedupe.Index).TrimEnd()} ({existingNumber + 1})";
            }
            else
            {
                targetPath = $"{targetPath} (1)";
            }
        }

        // File definitely doesn't exist. Serialize to JSON and save it.
        string jsonString = JsonSerializer.Serialize(keyFile);
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)); // Create intermediate directories if they don't exist
        await File.WriteAllTextAsync($"{targetPath}.json", jsonString, Encoding.UTF8);

        return $"{targetPath}.json";
    }

    public string[] GetList()
    {
        if (Directory.Exists(Path.Combine(BasePath, KeyfileFolderName)))
        {
            return Directory.GetFiles(Path.Combine(BasePath, KeyfileFolderName));
        }
        else
        {
            return Array.Empty<string>();
        }
    }

    public async Task<Dictionary<string, string>?> LoadKeyFile(string filePath)
    {
        // These are coming directly from the list, so all of these should be full filepaths
        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            string jsonString = await File.ReadAllTextAsync(filePath);
            var keymap = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
            return keymap;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to deserialize file {filePath}: {ex}");
            return null;
        }
    }
}

public interface IKeyFileService
{
    /// <summary>
    /// Get a list of all the JSON keyfiles in the keyfile folder.
    /// </summary>    
    string[] GetList();

    /// <summary>
    /// Save the given keyFile to the given filename in a folder dedicated just to keyfiles.
    /// </summary>    
    Task<string?> AddKeyFile(Dictionary<string, string> keyFile, string fileName);

    /// <summary>
    /// Load the given keyfile from disk and deserialize it into its dictionary form.
    /// </summary>
    /// <param name="filename"></param>    
    Task<Dictionary<string, string>?> LoadKeyFile(string filePath);
}

