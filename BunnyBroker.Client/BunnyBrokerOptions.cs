using Microsoft.AspNetCore.Http.Connections.Client;


namespace BunnyBroker.Client;

public class BunnyBrokerOptions {
    public string Url { get; set; } = "http://localhost:7191/bunnyhub";
    public string User { get; set; }
    public string Password { get; set; }
    public HttpConnectionOptions ConnectionOptions { get; set; } = new();
}
