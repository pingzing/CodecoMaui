namespace CodecoMaui.Models;

public class KeyFile
{
    public string FullPath { get; set; } = null!;
    public string FileName => Path.GetFileNameWithoutExtension(FullPath);
}

