using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace HomelabClient;

public class NetworkingClient
{
    /// <summary>
    /// The instance of the socket used for listening.
    /// </summary>
    private UdpClient _BroadcastListener;

    /// <summary>
    /// Whether the listener thread is running or not.
    /// </summary>
    private static volatile bool _Running = false;

    /// <summary>
    /// Instance for networking operations.
    /// </summary>
    public static NetworkingClient Instance {get; private set;}
    
    System.Threading.CancellationTokenSource _TaskCanceller;

    /// <summary>
    /// Called when the server gets data from a client.
    /// </summary>
    public event EventHandler<string>? NewItem;

    static NetworkingClient()
    {
        Instance = new NetworkingClient();
    }

    public NetworkingClient()
    {
        _BroadcastListener = new UdpClient(61000);
        _TaskCanceller = new System.Threading.CancellationTokenSource();
    }

    private void Reply(System.Net.IPEndPoint server)
    {
        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);
        bw.Write("Client 1.0.0.0");

        UdpClient _Client = new UdpClient();
        IPEndPoint serverListener = new IPEndPoint(server.Address, 60000);
        _Client.Connect(serverListener);
        _Client.Send(ms.GetBuffer());
    }


    private async void Listen()
    {

        _Running = true;
        try
        {
            while(_Running)
            {
                var data = await _BroadcastListener.ReceiveAsync(_TaskCanceller.Token);
                using MemoryStream ms = new MemoryStream(data.Buffer);
                using BinaryReader br = new BinaryReader(ms);
                string clientName = br.ReadString();
                Console.WriteLine("Got broadcast packet.");
                NewItem?.Invoke(this, clientName);
                Reply(data.RemoteEndPoint);
            }
        } catch(System.OperationCanceledException)
        {

        }
    }

    public void Start()
    {
        System.Threading.ThreadStart ts = new System.Threading.ThreadStart(Instance.Listen);
        System.Threading.Thread listenThread = new System.Threading.Thread(ts);
        listenThread.Start();
    }

    public void Stop()
    {
        _Running = false;
        _TaskCanceller?.Cancel();
    }

}