namespace Straonit.HighEdge.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TestController: ControllerBase{

    [HttpGet("test")]
    public string Test()=>"test";
}