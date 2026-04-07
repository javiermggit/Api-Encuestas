using Microsoft.AspNetCore.Mvc;

namespace DynamicSurvey.Api.Controllers.Public;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "API funcionando" });
    }
}