using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Email.API.BRL.Command
{
    public class EmailAttachment : Email
    {
        public IFormFile Attachment { get; set; }
    }
}