using System.ComponentModel.DataAnnotations;

namespace AppFinanzasWeb.Validaciones
{
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primeraLetra = value.ToString()[0].ToString();

            if (primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayúscula");
            }

            return ValidationResult.Success;
        }
    }

    public class LetrasMayusculasAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            foreach(char caracter in value.ToString())
            {
                if (caracter.ToString() != caracter.ToString().ToUpper())
                {
                    return new ValidationResult("Los caracteres deben ser todos en mayúsculas");
                }
            }

            return ValidationResult.Success;
        }
    }
}