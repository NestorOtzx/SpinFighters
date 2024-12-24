using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MatchMaking : ControllerBase
{
    private static HashSet<int> portsUsed = new HashSet<int>();
    private static List<Session> currentSessions = new List<Session>();

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

    [HttpGet("GetMatches")]
    public IActionResult GetAllMatches()
    {
        try{
            Console.WriteLine("Getting all matches");
            foreach(var match in currentSessions)
            {
                Console.WriteLine($"matchport:{match.port}");
            }

            string ans = JsonSerializer.Serialize(currentSessions);
            return Ok(ans);
        }catch(Exception e)
        {
            Console.WriteLine("ERROR AL OBTENER PARTIDAS");
            Console.WriteLine(e.Message);
            return StatusCode(500, $"Error: {e.Message}");
        }
    }

    private bool StartDedicatedServer(int port)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/home/ec2-user/SpinFighters/SpinBuilds/Server/spin.x86_64",
                Arguments = $"-batchmode -nographics \"-port {port}\"",
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
        
        

        Console.WriteLine($"Add port!! {port}");
        portsUsed.Add(port);
        foreach (var el in portsUsed)
        {
            Console.WriteLine($"Port used: {el}");
        }
        bool worked = process.Start();

        Session session = new Session("Game"+port.ToString(), port, process.Id);

        currentSessions.Add(session);
        
        process.Exited += (sender, e) => {
            currentSessions.Remove(session);
            portsUsed.Remove(port);
            Console.WriteLine($"BORRANDO EL PUERTO {port} de la lista!!");
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        return worked;
    }

    private static int FindAvailablePort()
    {
        const int startPort = 7777;
        const int endPort = 20000; // Rango de puertos disponibles

        for (int port = startPort; port <= endPort; port++)
        {
            if (!portsUsed.Contains(port))
            {
                return port;
            }
        }

        throw new Exception("No available ports!!");
    }


}
