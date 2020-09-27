using Microsoft.AspNetCore.Mvc;

namespace Nike.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Host is ready";
        }
    }
}