using System;
using System.ComponentModel.DataAnnotations;

namespace MarketAnalyzer.Captcha
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public sealed class CaptchaValidatorAttribute : ValidationAttribute
  {
    public override bool RequiresValidationContext => true;

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      var token = value as string;
      if (string.IsNullOrEmpty(token))
      {
        return ValidationResult.Success;
      }

      var validator = (ICaptchaValidator)validationContext.GetService(typeof(ICaptchaValidator));
      var result = validator.ValidateAsync(token).GetAwaiter().GetResult();
      if (result.Success)
      {
        return ValidationResult.Success;
      }

      return new ValidationResult("کد امنیتی معتبر نیست.");
    }
  }
}
