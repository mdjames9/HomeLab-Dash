using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace HomelabDash.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static

    public string Greeting => "Welcome to Avalonia!";


    public ObservableCollection<string> MyItems {get; set;}

    public MainWindowViewModel()
    {
        Networking.Instance.NewItem += NewItem;
        MyItems = new ObservableCollection<string>()
        {
            "test",
            "test2",
        };

    }

    public void NewItem(object? sender, string name)
    {
        if (!MyItems.Contains(name))
        {
            MyItems.Add(name);
        }  
    }

    public void Broadcast()
    {
        Networking.Instance.Broadcast();
    }


#pragma warning restore CA1822 // Mark members as static
}
