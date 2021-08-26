using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Nike.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public async Task<string> Get()
        {
            return "Host is ready .";
        }

        [HttpGet("message")]
        public async Task<HealthModel> GetMessage()
        {
            return new HealthModel("Host is ready .");
            // return Ok("Host is ready .");
        }
        
        [HttpGet("failed")]
        public async Task<HealthModel> Failed()
        {
            throw new InvalidDataException("for exception wrapping test");
        } 
        [HttpGet("null")]
        public async Task<HealthModel> NullReturn()
        {
            return null;
        }
    }

    public class HealthModel
    {
        public Guid Id { get; set; }
        public string Message { get; set; }

        public HealthModel(string message)
        {
            Id = Guid.NewGuid();
            Message = message;
        }
    }
}