using Microsoft.AspNetCore.Mvc;

namespace ProjectLogDotNet6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger_)
        {
            _logger = logger_;
        }
        [HttpGet]
        public string Test()
        {
            return "abc";
        }

        [Route("GetTest")]
        [HttpGet]
        public ActionResult GetTest()
        {
            _logger.LogInformation("TESTINGG FROM ENDPOINT");
            return Ok("TEST");
        }
    }
}
