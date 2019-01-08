using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Email.API.BRL.Command;
using Email.API.BRL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Email.API.Controllers
{
    [Authorize("EmailUser")]
    [Route("api")]
    [Consumes("application/json", "multipart/form-data")] //needed ???
    public class EmailController : BaseController, IController
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IEmailService _emailService;

        public EmailController(ILogger<EmailController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("emailanon")]
        public async Task<IActionResult> SendEmailAnonymous([FromBody] BRL.Command.Email email)
        {
            try
            {
                //add user email from token
                var result = await _emailService.SendMail(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogError(_logger, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost("email")]
        public async Task<IActionResult> SendEmail([FromBody] BRL.Command.Email email)
        {
            try
            {
                //add user email from token
                email.UserEmail = base.UserEmail;
                var result = await _emailService.SendMail(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogError(_logger, ex);
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost("emailattachment")]
        public async Task<IActionResult> SendEmailAttachment(BRL.Command.EmailAttachment email)
        {
            try
            {
                email.UserEmail = base.UserEmail;
                var result = await _emailService.SendMailAttachment(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogError(_logger, ex);
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}