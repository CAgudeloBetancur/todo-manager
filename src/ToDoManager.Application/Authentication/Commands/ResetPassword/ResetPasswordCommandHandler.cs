using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.DTOs;
using ToDoManager.Application.Common.Interfaces.Services;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider
        )
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<Unit>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken
        )
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user is null)
            return Errors.Authentication.InvalidCredentials;
        
        var resetResult = await _userRepository.ResetPasswordAsync(
            user, 
            request.ResetToken, 
            request.NewPassword
            );

        if (!resetResult.Succeeded)
            return MapToValidationErrors(resetResult.Errors);
        
        var existingTokens = await _refreshTokenRepository.GetByUserIdAsync(user.Id.Value);
        _refreshTokenRepository.InvalidateAsync(existingTokens);

        return Unit.Value;
    }

    private static List<Error> MapToValidationErrors(IEnumerable<AuthenticationError> errors)
    {
        return errors
            .Select(e => Error.Validation(e.Code, e.Description))
            .ToList();
    }
}
