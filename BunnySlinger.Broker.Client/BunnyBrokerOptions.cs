using Microsoft.AspNetCore.Http.Connections.Client;


namespace BunnySlinger.Broker.Client;

public class BunnyBrokerOptions {
    public string Url { get; set; } = "http://localhost:7191/bunnyhub";
    public HttpConnectionOptions ConnectionOptions { get; set; } = new();
}
