using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet("trigger-exception")]
    public IActionResult TriggerException()
    {
        throw new Exception("This is a simulated controller exception.");
    }

    [HttpGet("divide-by-zero")]
    public IActionResult DivideByZero()
    {
        int result = 10 / int.Parse("0");
        return Ok(result);
    }

    [HttpGet("invalid-operation")]
    public IActionResult InvalidOperation()
    {
        throw new InvalidOperationException("This is a simulated invalid operation exception.");
    }
}
