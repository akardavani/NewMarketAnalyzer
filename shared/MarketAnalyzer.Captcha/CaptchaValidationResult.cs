using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Captcha
{
  public class CaptchaValidationResult
  {
    private bool _succeeded;
    private IEnumerable<string> _errors;

    public virtual bool Success => _succeeded;

    public IReadOnlyCollection<string> Errors => _errors.ToArray();

    public void Succeeded()
    {
      _succeeded = true;
    }

    public void Failed(string error)
    {
      _succeeded = false;
      _errors = new[] { error };
    }

    public void Failed(IReadOnlyCollection<string> errors)
    {
      _succeeded = false;
      _errors = errors;
    }
  }
}
