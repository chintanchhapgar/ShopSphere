using MediatR;
using Microsoft.Extensions.Logging;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Authentication.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IBackgroundJobService _backgroundJobs;
    private readonly ILogger<RegisterCommandHandler> _logger;
    public RegisterCommandHandler(
        IIdentityService identityService,
        IBackgroundJobService backgroundJobs,
        ILogger<RegisterCommandHandler> logger)
    {
        _identityService = identityService;
        _backgroundJobs = backgroundJobs;
        _logger = logger;
    }

    public async Task<Result> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var response = await _identityService.RegisterAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        if (!response.Succeeded || response.UserId is null)
        {
            return Result.Failure(
                AuthenticationErrors.RegistrationFailed);
        }

        _logger.LogInformation(
            "User registered with email {Email}.",
            request.Email);

        var verification =
             await _identityService.GenerateEmailVerificationTokenAsync(
                 request.Email);

        if (verification is not null)
        {
            _backgroundJobs.Enqueue<IEmailJob>(
                x => x.SendEmailVerificationAsync(
                    verification.UserId,
                    verification.Token));
        }

        _logger.LogInformation(
            "Email verification queued for {Email}.",
            request.Email);        

        return Result.Success(
            "Registration completed successfully.");
    }
}