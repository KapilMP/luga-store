using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Settings;
using SedaWears.Domain.Enums;
using SedaWears.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Web;
using FluentValidation;

namespace SedaWears.Application.Features.Users.Commands;

public record InviteOwnerCommand(string Email) : IRequest;

public class InviteOwnerValidator : AbstractValidator<InviteOwnerCommand>
{
    public InviteOwnerValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Enter valid email address").EmailAddress().WithMessage("Enter valid email address");
    }
}

public class InviteOwnerHandler(
    UserManager<User> userManager,
    IEmailService emailService,
    AppConfig appConfig) : IRequestHandler<InviteOwnerCommand>
{
    public async Task Handle(InviteOwnerCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        
        if (user != null)
        {
             if (user.Role == UserRole.Owner)
             {
                 throw new BadRequestException("User is already an Owner.");
             }
             
             user.Role = UserRole.Owner;
             await userManager.UpdateAsync(user);
        }
        else
        {
            user = new User 
            { 
                Email = request.Email, 
                UserName = Guid.NewGuid().ToString(), 
                FirstName = "", 
                LastName = "",
                IsActive = true,
                Role = UserRole.Owner
            };
            
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded) 
                throw new BadRequestException(result.Errors.First().Description);
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}";

        await emailService.SendEmailAsync(
            user.Email!,
            "SedaWears Owner Invitation",
            $"<p>You have been invited as an Owner to SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>");
    }
}
