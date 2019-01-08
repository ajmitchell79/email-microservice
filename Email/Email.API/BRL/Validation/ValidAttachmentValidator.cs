using System.Text.RegularExpressions;
using FluentValidation.AspNetCore;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Http;

namespace Email.API.BRL.Validation
{
    public class ValidAttachmentValidator : PropertyValidator
    {
        public ValidAttachmentValidator() : base("Email attachment is not a valid file")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var file = ((IFormFile) context.PropertyValue);



            return (file != null) && (
                       file.Length > 0
           // && !string.IsNullOrEmpty(file.ContentType) 
            && !string.IsNullOrEmpty(file.FileName));
        }
    }
}