// See https://aka.ms/new-console-template for more information
using HomelabClient;

Console.WriteLine("Hello, World!");
NetworkingClient.Instance.Start();
Console.WriteLine("Listening. Press Enter to Stop.");
Console.ReadLine();
NetworkingClient.Instance.Stop();