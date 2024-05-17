using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Avalonia.Controls;
using Avalonia.Threading;

namespace HomelabDash.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static

    public string Greeting => "HomeLab Dash - The Computer Finder";


    public ObservableCollection<ClientInformation> MyItems {get; set;}

    public MainWindowViewModel()
    {
        if(!Design.IsDesignMode)
        {
            Networking.Instance.NewItem += NewItem;
        }
        MyItems = new ObservableCollection<ClientInformation>()
        {
            new ClientInformation()
            {
                Name="Test",
                OS = "FakeOS",
            }
        };
        MyItems[0].IPs.Add("127.0.0.1");
    }

    public void NewItem(object? sender, ClientInformation client)
    {
        Dispatcher.UIThread.InvokeAsync(()=>{
            bool found = false;
            foreach(ClientInformation existingClient in MyItems)
            {
                if(existingClient.Name.Equals(client.Name))
                {
                    found = true;
                }
            }
            if(!found)
            {
                MyItems.Add(client);
            }
        });
    }

    public void Broadcast()
    {
        MyItems.Clear();
        if(!Design.IsDesignMode)
        {
            Networking.Instance.Broadcast();
        }
    }


#pragma warning restore CA1822 // Mark members as static
}
