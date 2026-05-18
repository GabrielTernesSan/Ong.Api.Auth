using FluentValidation;
using MediatR;
using Ong.Commom;
using Ong.Domain.Enums;
using System.Text.Json.Serialization;

namespace Ong.Application.Requests;

public class RegisterRequest : IRequest<Response>
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Cpf { get; set; } = null!;
    public string Password { get; set; } = null!;
    [JsonIgnore]
    public string Role { get; set; } = ERole.Doador.ToString();
}

public class RegisterRequestValidator : FluentValidation.AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage("Nome é obrigatório.");

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Formato de email é inválido.");

        RuleFor(x => x.Cpf)
            .NotNull().WithMessage("CPF é obrigatório.")
            .WithMessage("Formato de CPF é inválido.");

        RuleFor(x => x.Password)
            .NotNull().WithMessage("Senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve conter no mínimo 6 caracteres.");
    }
}