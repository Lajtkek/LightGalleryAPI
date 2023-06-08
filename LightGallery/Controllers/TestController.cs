using Microsoft.AspNetCore.Mvc;

namespace LightGallery.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    public TestController()
    {
    }

    [HttpGet(Name = "Test")]
    public IActionResult Get()
    {
        return Ok();
    }
}