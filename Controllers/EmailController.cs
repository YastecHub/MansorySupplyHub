using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MansorySupplyHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService
                ?? throw new ArgumentNullException(nameof(emailService));
        }
        [HttpGet("singleemail")]
        public async Task<IActionResult> SendSingleEmail()
        
        {
            EmailMetadata emailMetadata = new("nayarohomolaso@gmail.com",
                "FluentEmail Test email",
                "This is a test email from FluentEmail.");
            await _emailService.Send(emailMetadata);
            return Ok();
        }
    }
}

