using Microsoft.AspNetCore.Mvc;

namespace OlympusVMS.Integration.Api.Controllers;

[ApiController]
[Route("s2s-fail-tests")]
public class S2sFailTestsController : ControllerBase
{
    [HttpPost("fail-500")]
    public IActionResult Fail500()
    {
        return StatusCode(StatusCodes.Status500InternalServerError, new
        {
            error = "Intentional failure for evaluation",
            status = StatusCodes.Status500InternalServerError
        });
    }
}
