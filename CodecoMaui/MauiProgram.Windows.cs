using Microsoft.Maui.LifecycleEvents;

namespace CodecoMaui;

public static class MauiProgram_Windows
{
    /// <summary>
    /// Sets up Windows-specific configuration.
    /// </summary>
    public static ILifecycleBuilder ConfigureWindows(ILifecycleBuilder builder)
    {
#if WINDOWS
        builder.AddWindows(win =>
        {
            win.OnWindowCreated(created => created.Title = "Codeco Maui Edition");
        });
#endif
        return builder;
    }
}

