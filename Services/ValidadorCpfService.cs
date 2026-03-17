using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ConsultorioApi.Services
{
    public class ValidadorCpfService : ValidationAttribute
    {

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var cpf = value as string;
            if (string.IsNullOrEmpty(cpf))
                return new ValidationResult("O CPF é obrigatório.");

            cpf = Regex.Replace(cpf, @"[^\d]", "");

            if (cpf.Length != 11)
                return new ValidationResult("O CPF deve conter exatamente 11 dígitos.");

            return ValidationResult.Success;
        }
    }
}