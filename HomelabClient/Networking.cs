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
    CancellationTokenSource _TaskCanceller;

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
        string hostname = System.Net.Dns.GetHostName();
        bw.Write(hostname);
        bw.Write(Environment.OSVersion.ToString());
        var ipList = GetLocalIPAddress(hostname);
        bw.Write(ipList.Count);
        foreach(string ip in ipList)
        {
            bw.Write(ip);
        }

        UdpClient _Client = new UdpClient();
        IPEndPoint serverListener = new IPEndPoint(server.Address, 60000);
        _Client.Connect(serverListener);
        _Client.Send(ms.GetBuffer());
    }

    public static List<string> GetLocalIPAddress(string hostname)
    {
        List<string> ips = new List<string>();
        var host = Dns.GetHostEntry(hostname);
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ips.Add(ip.ToString());
            }
        }
        return ips;
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