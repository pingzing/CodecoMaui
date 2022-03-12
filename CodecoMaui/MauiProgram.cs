using CodecoMaui.Services;
using Microsoft.Maui.LifecycleEvents;
using static CodecoMaui.MauiProgram_Windows;

namespace CodecoMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureLifecycleEvents(events =>
            {
				ConfigureWindows(events);
			})
			.Services.AddSingleton<IKeyFileService, KeyFileService>();

		return builder.Build();
	}
}
