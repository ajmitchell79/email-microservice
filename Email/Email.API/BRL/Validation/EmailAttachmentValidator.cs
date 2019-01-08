using System.Collections.Generic;
using FluentValidation;

namespace Email.API.BRL.Validation
{
    public class EmailAttachmentValidator : AbstractValidator<Command.EmailAttachment>
    {
        public EmailAttachmentValidator()
        {
            RuleForEach(m => m.To).NotEmpty().ValidEmail();
            RuleFor(m => m.From).NotEmpty().ValidEmail();
            RuleFor(m => m.Subject).NotEmpty();

            RuleFor(m => m.Attachment).ValidAttachment();
        }
    }
}