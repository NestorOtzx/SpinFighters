using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MatchMaking : ControllerBase
{
    private readonly ILogger<MatchMaking> _logger;

    public MatchMaking(ILogger<MatchMaking> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ContentResult Get()
    {
        return base.Content("<div>Hello world</div>", "text/html");
    }

    [HttpGet("CreateMatchTest")]
    public IActionResult CreateMatch2()
    {
        return Ok("RESPUESTAA");
    }

    [HttpGet("CreateMatch")]
    public async Task CreateMatch()
    {
        try{
            int port = FindAvailablePort();
            Console.WriteLine($"Puerto encontrado: {port}");
            bool worked = StartDedicatedServer(port);
            Console.WriteLine($"Server iniciado? {worked}");
        }catch(Exception e)
        {
            Console.Write("ERROR AL CREAR PARTIDA");
            Console.WriteLine(e.Message);
        }
    }

    private bool StartDedicatedServer(int port)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/home/ec2-user/SpinFighters/SpinBuilds/Server/spin.x86_64",
                Arguments = $"-batchmode -nographics '-port {port}'",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        return process.Start();
    }

    private bool IsPortAvailable(int port)
    {
        try
        {
            using (var tcpListener = new TcpListener(IPAddress.Any, port))
            {
                tcpListener.Start();
                tcpListener.Stop();
                return true;
            }
        }
        catch (SocketException)
        {
            return false; // El puerto está ocupado
        }
    }

    private int FindAvailablePort()
    {
        const int startPort = 7777;
        const int endPort = 20000; // Rango de puertos disponibles

        for (int port = startPort; port <= endPort; port++)
        {
            if (IsPortAvailable(port))
            {
                return port;
            }
        }
        throw new Exception("No available ports!!");
    }
}