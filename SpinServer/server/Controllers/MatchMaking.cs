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
            var process = StartDedicatedServer(port);
            Console.WriteLine("Server iniciado en paralelo en el puerto: ", port);

            await process;

            Console.WriteLine($"Servidor en puerto {port} cerrado: ");

        }catch(Exception e)
        {
            Console.Write("ERROR AL CREAR PARTIDA");
            Console.WriteLine(e.Message);
        }
    }

    private async Task StartDedicatedServer(int port)
    {
        await Task.Run(() => {
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

            process.Start();

            // Leer salida del proceso para verificar errores
            string output = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(output))
            {
                throw new Exception($"Server error: {output}");
            }
        });
        
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