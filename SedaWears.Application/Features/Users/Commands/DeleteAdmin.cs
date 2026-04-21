using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Users.Commands;

public record DeleteAdminCommand(int Id) : IRequest;

public class DeleteAdminValidator : AbstractValidator<DeleteAdminCommand>
{
    public DeleteAdminValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class DeleteAdminHandler(UserManager<User> userManager, IUserCuckooFilter cuckooFilter) : IRequestHandler<DeleteAdminCommand>
{
    public async Task Handle(DeleteAdminCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());

        if (user is null || user.Role != UserRole.Admin)
            throw new NotFoundException("Admin not found.");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);

        await cuckooFilter.RemoveAsync(user.Email!, UserRole.Admin);
    }
}
