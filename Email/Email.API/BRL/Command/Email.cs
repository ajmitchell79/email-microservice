using System.Collections.Generic;

namespace Email.API.BRL.Command
{
    public class Email
    {
        public List<string> To { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public bool IsHtml { get; set; }

        public string UserEmail { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }
    }
}