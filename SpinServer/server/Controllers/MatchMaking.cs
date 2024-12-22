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
    [HttpGet]
    public ContentResult Get()
    {
        return base.Content("<div>Hello world</div>", "text/html");
    }

    [HttpGet("CreateMatch")]
    public IActionResult CreateMatch()
    {
        int port = FindAvailablePort();
        if (port == -1) return StatusCode(500, "No available ports");

        var process = StartDedicatedServer(port);

        Console.WriteLine("SERVIDOR INICIADO DESDE BACKEND EN EL PUERTO "+port+ " pid: "+process.Id);

        return Ok("Partida creada con exito");
    }

    private Process StartDedicatedServer(int port)
    {
        try
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

            process.Start();

            // Leer salida del proceso para verificar errores
            string output = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine($"Server error: {output}");
                return null;
            }

            return process;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting server: {ex.Message}");
            return null;
        }
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
        return -1; // No hay puertos disponibles
    }
}