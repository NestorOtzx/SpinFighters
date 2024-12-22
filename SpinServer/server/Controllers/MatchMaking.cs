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
    public string CreateMatch()
    {
        return "<h1>create match</h1>";
    }
}