using System.Threading.Tasks;
using Email.API.BRL.Command;

namespace Email.API.BRL.Interfaces
{
    public interface IEmailService
    {
        Task<EmailResponse> SendMail(BRL.Command.Email email);

        Task<EmailResponse> SendMailAttachment(EmailAttachment email);

    }
}