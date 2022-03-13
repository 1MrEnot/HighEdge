namespace Straonit.HighEdge.Controllers;

using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
public class TestController : Controller
{
    public IActionResult Index()
    {
        return View(null);
    }
}