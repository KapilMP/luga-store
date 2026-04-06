using System.Web;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;
using LugaStore.Application.Common.Settings;

namespace LugaStore.Application.Features.Auth.Commands;

public record RegisterCommand(string Email, string Password, string FirstName, string LastName, string Phone) : IRequest<bool>;

public class RegisterCommandHandler(
    UserManager<User> userManager,
    IEmailSender emailSender,
    AppConfig appConfig) :
    IRequestHandler<RegisterCommand, bool>
{
    private string FrontendUrl => appConfig.FrontendUrl;

    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing != null)
        {
            if (existing.PasswordHash != null) return false;
            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.PhoneNumber = request.Phone;
            await userManager.UpdateAsync(existing);
            var addPasswordResult = await userManager.AddPasswordAsync(existing, request.Password);
            if (!addPasswordResult.Succeeded) return false;
            await SendVerificationEmailAsync(request.Email);
            return true;
        }

        var user = new User
        {
            Email = request.Email,
            UserName = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.Phone,
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return false;

        await userManager.AddToRoleAsync(user, Roles.Customer);
        await SendVerificationEmailAsync(request.Email);
        return true;
    }

    private async Task SendVerificationEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return;
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var verificationUrl = $"{FrontendUrl}/verify-email?userId={user.Id}&token={encodedToken}";
        await emailSender.SendEmailAsync(email, "Verify Your email", $"Please verify your account by clicking here: {verificationUrl}");
    }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
        RuleFor(v => v.Password).NotEmpty().MinimumLength(8);
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.Phone).NotEmpty().Matches(@"^\+?[0-9]{7,15}$").WithMessage("A valid phone number is required.");
    }
}
