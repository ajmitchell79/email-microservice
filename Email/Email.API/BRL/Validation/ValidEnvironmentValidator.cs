using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentValidation.Validators;

namespace Email.API.BRL.Validation
{
    public class ValidEnvironmentValidator : PropertyValidator
    {
        private static readonly List<string> environments = new List<string> { "Development", "Production" };

        public ValidEnvironmentValidator() : base("Environment '{PropertyValue}' is not valid, environment must be either:" 
                                                  + string.Join(", ", environments))
        { 
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            return environments.Contains((string) context.PropertyValue);
        }
    }
}