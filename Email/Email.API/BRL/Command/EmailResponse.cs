using System;

namespace Email.API.BRL.Command
{
    public class EmailResponse
    {
        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string From { get; set; }


        public int ContentLength { get; set; }

        public DateTime Sent { get; set; }

        public string Environment { get; set; }


        public Attachment Attachment { get; set; }
    }

    public class Attachment
    {
        public string Name { get; set; }
        public string ContentType { get; set; }

        public long ContentLength { get; set; }
    }

}