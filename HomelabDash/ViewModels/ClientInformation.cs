using System.Collections.Generic;
using System.Net;
using HomelabDash.ViewModels;

public class ClientInformation : ViewModelBase
{
    public string Name {get; set;}
    public string OS {get; set;}

    public string ResponseFrom {get; set;}
    public List<string> IPs {get; private set;}

    public ClientInformation()
    {
        Name = string.Empty;
        OS = string.Empty;
        ResponseFrom = IPAddress.Any.ToString();
        IPs = new List<string>();
    }
}