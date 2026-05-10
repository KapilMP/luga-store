using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Users.Commands;

public record DeleteAdminCommand(int Id) : IRequest;

public class DeleteAdminValidator : AbstractValidator<DeleteAdminCommand>
{
    public DeleteAdminValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class DeleteAdminHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext) : IRequestHandler<DeleteAdminCommand>
{
    public async Task Handle(DeleteAdminCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && u.Role == UserRole.Admin, ct)
            ?? throw new NotFoundException("User not found.");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);

    }
}
