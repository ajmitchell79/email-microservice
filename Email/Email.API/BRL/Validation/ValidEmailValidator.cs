using System.Text.RegularExpressions;
using FluentValidation.Validators;

namespace Email.API.BRL.Validation
{
    public class ValidEmailValidator : PropertyValidator
    {
        public ValidEmailValidator() : base("Email address '{PropertyValue}' is not a valid email")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
           return new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")
               .Match((string)context.PropertyValue)
               .Success;
        }
    }
}