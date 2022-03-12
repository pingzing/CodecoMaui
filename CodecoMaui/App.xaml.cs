using CodecoMaui.Pages;
using CodecoMaui.Services;

namespace CodecoMaui;

public partial class App : Application
{
    private readonly IKeyFileService _keyFileService;

    public App(IKeyFileService keyFileService)
	{
		InitializeComponent();

        _keyFileService = keyFileService;
		MainPage = new MainPage(_keyFileService);
    }
}
