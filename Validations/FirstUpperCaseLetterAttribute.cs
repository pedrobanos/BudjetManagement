using System.ComponentModel.DataAnnotations;

namespace BudjetManagement.Validations
{
    public class FirstUpperCaseLetterAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) {
                return ValidationResult.Success;
            }

            var firstLetter = value.ToString()[0].ToString();
            if(firstLetter!= firstLetter.ToUpper())
            {
                return new ValidationResult("First Letter must be UpperCase");
            }
            return ValidationResult.Success;    
        }
    }
}
