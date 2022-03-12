using System.Net;
using hackation_high_edge.Extensions;
using hackation_high_edge.Models;
using hackation_high_edge.Service;
using Microsoft.AspNetCore.Mvc;

namespace hackation_high_edge.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController: ControllerBase{

    [HttpGet("test")]
    public string Test()=>"test";
}