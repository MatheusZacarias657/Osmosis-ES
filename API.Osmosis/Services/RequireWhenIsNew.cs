using System.ComponentModel.DataAnnotations;

namespace Osmosis.Services
{
    public class RequireWhenIsNew : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            object obj = validationContext.ObjectInstance;
            string id = obj.GetType().GetProperty("id").GetValue(obj).ToString();

            if (!id.Equals("0"))
                return ValidationResult.Success;

            string prop = string.Empty; 

            try
            {
                prop = value.ToString();
            }
            catch { }
            
            return string.IsNullOrEmpty(prop) ? new ValidationResult("Value is required.") : ValidationResult.Success;
        }
    }
}
