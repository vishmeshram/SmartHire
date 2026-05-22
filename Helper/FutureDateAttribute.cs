using System.ComponentModel.DataAnnotations;

namespace SmartHire.Helpers
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return true; // nullable, so null is fine
            if (value is DateTime date)
                return date.Date >= DateTime.UtcNow.Date;
            return false;
        }
    }
}
