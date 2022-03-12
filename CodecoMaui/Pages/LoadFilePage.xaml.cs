using CodecoMaui.Messages;
using CodecoMaui.Models;
using CodecoMaui.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CodecoMaui.Pages;

public partial class LoadFilePage : ContentPage
{
    private readonly MainPage _mainPageRef;
    private readonly IKeyFileService _keyFileService;

    private ObservableCollection<KeyFile> _keyFiles = new ObservableCollection<KeyFile>();

    public LoadFilePage(MainPage mainPageRef, IKeyFileService keyFileService)
	{
        _mainPageRef = mainPageRef;
        _keyFileService = keyFileService;
		InitializeComponent();

        SavedKeyFilesCollectionView.ItemsSource = _keyFiles;
    }

    protected override void OnAppearing()
    {
        _keyFiles.Clear();        
        foreach (KeyFile keyfile in _keyFileService.GetList().Select(x => new KeyFile { FullPath = x }))
        {
            AddToCollectionSorted(keyfile);
        }
    }

    private async void LoadSelectedButton_Clicked(object sender, EventArgs e)
    {
        var keyFile = (SavedKeyFilesCollectionView.SelectedItem as KeyFile);
        if (keyFile == null)
        {
            return;
        }

        Dictionary<string, string>? keymap = await _keyFileService.LoadKeyFile(keyFile.FullPath);
        if (keymap == null)
        {
            return;
        }

        MessagingCenter.Send(this, nameof(FileLoadedMessage), new FileLoadedMessage { LoadedKeyMap = keymap, FileName = keyFile.FileName });
        await _mainPageRef.Navigation.PopAsync();
    }

    private void SavedKeyFilesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Any())
        {
            LoadSelectedButton.IsEnabled = true;
        }
        else
        {
            LoadSelectedButton.IsEnabled = false;
        }
    }

    private async void AddNewButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            FileResult? result = await FilePicker.PickAsync(new PickOptions { PickerTitle = "Pick a key file" });
            if (result != null)
            {
                Dictionary<string, string>? keymap = await ParseFile(result);
                if (keymap != null)
                {
                    string? fileName = await DisplayPromptAsync("New Keyfile name", "What would you like to name this keyfile?", initialValue: "New Keyfile");
                    if (fileName == null)
                    {
                        return;
                    }

                    string? resolvedFilePath = await _keyFileService.AddKeyFile(keymap, fileName);
                    if (resolvedFilePath == null)
                    {
                        return;
                    }

                    AddToCollectionSorted(new KeyFile { FullPath = resolvedFilePath });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to open the given file. Exception: {ex}");
        }
    }

    private async Task<Dictionary<string, string>?> ParseFile(FileResult result)
    {
        Dictionary<string, string> keymap = new Dictionary<string, string>();

        try
        {
            using Stream stream = await result.OpenReadAsync();
            using StreamReader reader = new StreamReader(stream);

            while (reader.Peek() >= 0)
            {
                string? line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                // Assume pipe, i.e. this charater: | is being used as a separator.
                if (!line.Contains('|'))
                {
                    Debug.WriteLine($"Error: Line did not contain a pipe. Skipping. Text in line: {line}");
                    continue;
                }

                string[] halves = line.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (halves.Any(x => string.IsNullOrWhiteSpace(x)))
                {
                    Debug.WriteLine($"Error: One half of the line was empty or whitespace. Skipping. Text in line: {line}");
                    continue;
                }

                keymap.Add(halves[0], halves[1]);
            }

            return keymap;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to parse the given input file. Details: {ex}");
            return null;
        }
    }

    private void AddToCollectionSorted(KeyFile keyfile)
    {
        if (_keyFiles.Count == 0)
        {
            _keyFiles.Add(keyfile);
            return;
        }

        var comparer = Comparer<string>.Default;
        int targetIndex = 0;
        while (targetIndex < _keyFiles.Count && comparer.Compare(_keyFiles[targetIndex].FileName, keyfile.FileName) < 0)
        {
            targetIndex++;
        }

        _keyFiles.Insert(targetIndex, keyfile);
    }
}