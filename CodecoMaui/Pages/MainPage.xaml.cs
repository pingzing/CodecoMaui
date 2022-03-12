using CodecoMaui.Services;

namespace CodecoMaui.Pages;

public partial class MainPage : NavigationPage
{
	private readonly IKeyFileService _keyFileService;

	public MainPage(IKeyFileService keyFileService)
	{
        _keyFileService = keyFileService;
		InitializeComponent();
		Navigation.PushAsync(new CodeLookupPage(this, _keyFileService));
    }
}

