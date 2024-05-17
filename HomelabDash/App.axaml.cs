using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using HomelabDash.ViewModels;
using HomelabDash.Views;

namespace HomelabDash;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        if(!Design.IsDesignMode)
        {
            Networking.Initialize();
            Networking.Instance.Start();
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
            desktop.MainWindow.Closing += Closing;
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void Closing(object? sender, EventArgs e)
    {
        if(!Design.IsDesignMode)
        {
            Networking.Instance.Stop();
        }
    }
}