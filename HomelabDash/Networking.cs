using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace HomelabDash;

public class Networking
{
    /// <summary>
    /// The instance of the socket used for listening.
    /// </summary>
    private UdpClient _Listener;

    /// <summary>
    /// Whether the listener thread is running or not.
    /// </summary>
    private static volatile bool _Running = false;

    /// <summary>
    /// Instance for networking operations.
    /// </summary>
    public static Networking Instance {get; private set;}
    
    System.Threading.CancellationTokenSource _TaskCanceller;

    /// <summary>
    /// Called when the server gets data from a client.
    /// </summary>
    public event EventHandler<string>? NewItem;

    static Networking()
    {
        Instance = new Networking();
    }

    public Networking()
    {
        _Listener = new UdpClient(60000);
        _TaskCanceller = new System.Threading.CancellationTokenSource();
    }

    public void Broadcast()
    {
        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);
        bw.Write("HD 1.0.0.0");

        System.Net.IPEndPoint broadcastDestination = new IPEndPoint(IPAddress.Broadcast, 61000);
        UdpClient _Broadcaster = new UdpClient();
        _Broadcaster.EnableBroadcast = true;
        _Broadcaster.Connect(broadcastDestination);
        _Broadcaster.Send(ms.GetBuffer());
    }


    private async void Listen()
    {

        _Running = true;
        try
        {
            while(_Running)
            {
                var data = await _Listener.ReceiveAsync(_TaskCanceller.Token);
                using MemoryStream ms = new MemoryStream(data.Buffer);
                using BinaryReader br = new BinaryReader(ms);
                string clientName = br.ReadString();
                Console.WriteLine("Got reply packet.");
                NewItem?.Invoke(this, clientName);
            }
        }
        catch (System.OperationCanceledException)
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