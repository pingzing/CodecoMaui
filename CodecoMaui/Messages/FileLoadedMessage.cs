namespace CodecoMaui.Messages;

internal class FileLoadedMessage
{
    public Dictionary<string, string> LoadedKeyMap { get; set; }
    public string FileName { get; set; }
}

