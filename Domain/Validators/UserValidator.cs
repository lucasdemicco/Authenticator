using Domain.Entity;
using FluentValidation;

namespace Domain.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.UserName)
         .NotEmpty().WithMessage("Name do usuário é obrigatório.")
         .WithErrorCode("ERR: User UserName Property")
         .MinimumLength(3).WithMessage("Nome deve conter no mínimo 6 caracteres.");

        RuleFor(x => x.PasswordHash)
         .NotEmpty().WithMessage("Senha do usuário é obrigatório.")
         .WithErrorCode("ERR: User PasswordHash Property")
         .MinimumLength(6).WithMessage("Senha deve conter no mínimo 6 caracteres.");
    }
}
