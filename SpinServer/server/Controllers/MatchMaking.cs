using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MatchMaking : ControllerBase
{
    [HttpGet("CreateGame")]
    public string Get()
    {
        return "Crear partida";
    }
}