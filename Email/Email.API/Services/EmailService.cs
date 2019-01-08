using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Email.API.BRL.Command;
using Email.API.BRL.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Attachment = System.Net.Mail.Attachment;

namespace Email.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly BRL.Common.SmtpSettings _emailConfiguration;

        public EmailService(IOptions<BRL.Common.SmtpSettings> emailConfiguration)
        {
            this._emailConfiguration = emailConfiguration.Value;
        }

        public async Task<EmailResponse> SendMail(BRL.Command.Email email)
        {
            //if dev environment send the email back to the user
            var msg = new MailMessage(
                // to: isDevEnvironment() ? email.UserEmail : string.Join(",", email.To.ToArray()),
                //to: (isDevEnvironment() && !anonymous) ? email.UserEmail : string.Join(",", email.To.ToArray()),
                to: isDevEnvironment() ? (email.UserEmail ?? string.Join(",", email.To.ToArray()))  : string.Join(",", email.To.ToArray()),
                from: email.From,
                
                subject: email.Subject,
                //body: isDevEnvironment() ? "DEV: redirect to " +  email.UserEmail + "<br> " + email.Message : email.Message
                body: isDevEnvironment() ? "DEV: redirect to " + (email.UserEmail ?? string.Join(",", email.To.ToArray())) + "<br> " + email.Message : email.Message
            )
            {
                IsBodyHtml = email.IsHtml                
            };

            if (!string.IsNullOrEmpty(email.Cc)) msg.CC.Add(new MailAddress(email.Cc));


            if (!string.IsNullOrEmpty(email.Bcc)) msg.Bcc.Add(new MailAddress(email.Bcc));

            using (var client = new SmtpClient())
            {
                client.Port = _emailConfiguration.SmtpPort;
                client.Host = _emailConfiguration.SmtpHost;
                await client.SendMailAsync(msg);
                return new EmailResponse()
                {
                    From = email.From,
                    To = isDevEnvironment() ? "(DEV env redirect) " + string.Join(",", email.To.ToArray()) : string.Join(",", email.To.ToArray()),
                    Cc = !string.IsNullOrEmpty(email.Cc) ? email.Cc : "",
                    Bcc = !string.IsNullOrEmpty(email.Bcc) ? email.Bcc : "",
                    ContentLength = email.Message.Length,
                    Sent = DateTime.Now,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                };
            }
        }

        public async Task<EmailResponse> SendMailAttachment(EmailAttachment email)
        {
            //if dev environment send the email back to the user
            var msg = new MailMessage(
                to: isDevEnvironment() ? email.UserEmail : string.Join(",", email.To.ToArray()),
                from: email.From);
            msg.Subject = email.Subject;
            msg.Body = email.Message;
            msg.IsBodyHtml = email.IsHtml;

            if (!string.IsNullOrEmpty(email.Cc)) msg.CC.Add(new MailAddress(email.Cc));
            if (!string.IsNullOrEmpty(email.Bcc)) msg.Bcc.Add(new MailAddress(email.Bcc));

            string fileName = Path.GetFileName(email.Attachment.FileName);

            using (var reader = new StreamReader(email.Attachment.OpenReadStream()))
            {
                msg.Attachments.Add(new Attachment(reader.BaseStream, fileName));

                using (var client = new SmtpClient())
                {
                    client.Port = _emailConfiguration.SmtpPort;
                    client.Host = _emailConfiguration.SmtpHost;
                    await client.SendMailAsync(msg);

                    return new EmailResponse()
                    {
                        From = email.From,
                        To = isDevEnvironment() ? "(DEV env redirect) " + string.Join(",", email.To.ToArray()) : string.Join(",", email.To.ToArray()),
                        Cc = !string.IsNullOrEmpty(email.Cc) ? email.Cc : "",
                        Bcc = !string.IsNullOrEmpty(email.Bcc) ? email.Bcc : "",
                        Sent = DateTime.Now,
                        ContentLength = email.Message.Length,
                        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                        Attachment = new BRL.Command.Attachment()
                        {
                            ContentLength = email.Attachment.Length,
                            Name = email.Attachment.FileName,
                            ContentType = email.Attachment.ContentType
                        }

                    };

                }
            }
        }

        private bool isDevEnvironment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        }
        

    }

   
}