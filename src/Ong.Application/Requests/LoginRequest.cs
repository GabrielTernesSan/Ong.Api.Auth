using FluentValidation;
using MediatR;
using Ong.Commom;

namespace Ong.Application.Requests;

public class LoginRequest : IRequest<Response>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Formato de email é inválido.");

        RuleFor(x => x.Password)
            .NotNull().WithMessage("Senha é obrigatória.");
    }
}