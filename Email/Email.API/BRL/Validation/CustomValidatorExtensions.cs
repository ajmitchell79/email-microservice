using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Email.API.BRL.Validation
{
    public static class CustomValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new ValidEmailValidator());
        }

        public static IRuleBuilderOptions<T, string> ValidEnvironment<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new ValidEnvironmentValidator());
        }

        public static IRuleBuilderOptions<T, IFormFile> ValidAttachment<T>(this IRuleBuilder<T, IFormFile> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new ValidAttachmentValidator());
        }
    }
}