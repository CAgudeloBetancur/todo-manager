using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Services;

namespace ToDoManager.Application.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<ErrorOr<Unit>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        
        if (user is null)
            return Unit.Value;

        var resetToken = await _userRepository.GeneratePasswordResetTokenAsync(user);

        await _emailService.SendPasswordResetEmailAsync(user.Email.Value, resetToken, cancellationToken);

        return Unit.Value;
    }
}
