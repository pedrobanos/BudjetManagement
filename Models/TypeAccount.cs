using BudjetManagement.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BudjetManagement.Models
{
    public class TypeAccount //: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is absolutely required")]
        [FirstUpperCaseLetter]
        [Remote(action: "VerifyExistTypeAccount", controller:"TypesAccounts")]

        public string Name { get; set; }
        public int UserId { get; set; }
        public int OrderNumber { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Name != null && Name.Length > 0)
        //    {
        //        var firstLetter = Name[0].ToString();

        //        if (firstLetter != firstLetter.ToUpper())
        //        {
        //            yield return new ValidationResult("The first Letter must be in UpperCase", new[] { nameof(Name) });
        //        }
        //    }
        //}

        /*Pruebas de otras validaciones*/
        //[Required(ErrorMessage ="The field {0} is required")]
        //[EmailAddress(ErrorMessage ="Error email form")]
        //public string Email { get; set; }
        //[Range(minimum:18,maximum:130, ErrorMessage ="The value need to be between {1} and {2}")]
        //public int Age { get; set; }
        //[Url(ErrorMessage ="URL format must be valid")]
        //public string URL { get; set; }
        //[CreditCard(ErrorMessage ="Must be a valid credit card format")]
        //public string CreditCard { get; set; }

    }
}
