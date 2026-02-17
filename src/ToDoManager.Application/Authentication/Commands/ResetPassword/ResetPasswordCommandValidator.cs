using FluentValidation;

namespace ToDoManager.Application.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.");

        RuleFor(c => c.ResetToken)
            .NotEmpty()
            .WithMessage("Reset token is required.");

        RuleFor(c => c.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.");
    }
}
