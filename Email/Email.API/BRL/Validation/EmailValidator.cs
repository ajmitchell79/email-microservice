using System.Collections.Generic;
using FluentValidation;

namespace Email.API.BRL.Validation
{
    public class EmailValidator : AbstractValidator<Command.Email>
    {
        public EmailValidator()
        {
            RuleForEach(m => m.To).NotEmpty().ValidEmail();
            RuleFor(m => m.From).NotEmpty().ValidEmail();
            RuleFor(m => m.Subject).NotEmpty();
        }
    }
}