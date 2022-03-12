using CodecoMaui.Messages;
using CodecoMaui.Pages;
using CodecoMaui.Services;

namespace CodecoMaui;

public partial class CodeLookupPage : ContentPage
{
	private readonly IKeyFileService _keyFileService;

	private MainPage _mainPageRef;
	private Dictionary<string, string> _keyCodeLookupMap = new Dictionary<string, string>();
	private string? _openedFileName = null;

    public CodeLookupPage(MainPage mainPageRef, IKeyFileService keyFileService)
    {
        _mainPageRef = mainPageRef;
        MessagingCenter.Subscribe<LoadFilePage, FileLoadedMessage>(this, nameof(FileLoadedMessage), FileLoaded);
        _keyFileService = keyFileService;
        InitializeComponent();
    }

    private void KeyInputEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue == null)
        {
            return;
        }

        if (!_keyCodeLookupMap.TryGetValue(e.NewTextValue, out string? foundValue))
        {
            CopyButton.IsEnabled = false;
            FoundCodeLabel.Text = "";
            return;
        }

        CopyButton.IsEnabled = true;
        FoundCodeLabel.Text = foundValue;
    }

    private async void LoadFileButton_Clicked(object sender, EventArgs e)
    {
		await _mainPageRef.PushAsync(new LoadFilePage(_mainPageRef, _keyFileService));
    }

    private async void CopyButton_Clicked(object sender, EventArgs e)
    {
        await Clipboard.SetTextAsync(FoundCodeLabel.Text);
    }

    private void FileLoaded(LoadFilePage sender, FileLoadedMessage args)
	{
        _keyCodeLookupMap = args.LoadedKeyMap;
        KeyInputEditor_TextChanged(sender, new TextChangedEventArgs(KeyInputEditor.Text, KeyInputEditor.Text));
        Title = $"Code Lookup - {args.FileName}";
	}
}