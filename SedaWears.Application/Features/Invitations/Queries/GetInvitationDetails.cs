using SedaWears.Application.Features.Invitations.Models;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Invitations.Queries;

public record GetInvitationDetailsQuery(string Email, string Token) : IRequest<InvitationDetailsResponse>;

public class GetInvitationDetailsHandler(
    UserManager<User> userManager) : IRequestHandler<GetInvitationDetailsQuery, InvitationDetailsResponse>
{
    public async Task<InvitationDetailsResponse> Handle(GetInvitationDetailsQuery request, CancellationToken ct)
    {
        var users = await userManager.Users
            .Where(u => u.Email == request.Email)
            .ToListAsync(ct);

        foreach (var user in users)
        {
            var isValid = await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", request.Token);
            if (isValid)
            {
                return new InvitationDetailsResponse(user.Email!, user.Role.ToString());
            }
        }

        throw new BadRequestException("Invalid or expired invitation token.");
    }
}
