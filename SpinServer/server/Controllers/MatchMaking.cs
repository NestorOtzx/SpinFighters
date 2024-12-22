using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
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
    public IActionResult CreateMatch()
    {
        try{
            int fport = FindAvailablePort();
            Console.WriteLine($"Puerto encontrado: {fport}");
            bool worked = StartDedicatedServer(fport);
            Console.WriteLine($"Server iniciado? {worked}");
            var ans =  new {
                Message = "Server started",
                port = fport
            };
            return Ok(ans);
        }catch(Exception e)
        {
            var ans =  new {
                Message = "Something went wrong on server!",
                port = -1
            };
            Console.Write("ERROR AL CREAR PARTIDA");
            Console.WriteLine(e.Message);
            return StatusCode(500, ans);
        }
    }

    private bool StartDedicatedServer(int port)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/home/ec2-user/SpinFighters/SpinBuilds/Server/spin.x86_64",
                Arguments = $"-batchmode -nographics \"-port {port}\" -logfile serverlog.txt",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"[OUTPUT] {e.Data}");
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"[ERROR] {e.Data}");
            }
        };
        bool worked = process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        return worked;
    }

    static bool IsUdpPortInUse(int port)
    {
        try
        {
            using (UdpClient client = new UdpClient(port))
            {
                // Intentar vincular el puerto UDP
                client.Close();
            }
            return false; // El puerto no está en uso
        }
        catch (SocketException)
        {
            return true; // El puerto está en uso
        }
    }

    private int FindAvailablePort()
    {
        const int startPort = 7777;
        const int endPort = 20000; // Rango de puertos disponibles

        for (int port = startPort; port <= endPort; port++)
        {
            if (IsUdpPortInUse(port))
            {
                return port;
            }
        }
        throw new Exception("No available ports!!");
    }
}