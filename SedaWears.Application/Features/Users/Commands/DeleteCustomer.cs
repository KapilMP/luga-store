using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Users.Commands;

public record DeleteCustomerCommand(int Id) : IRequest;

public class DeleteCustomerValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid customer identifier is required.");
    }
}

public class DeleteCustomerHandler(
    UserManager<User> userManager, 
    IApplicationDbContext dbContext,
    IUserCuckooFilter cuckooFilter) : IRequestHandler<DeleteCustomerCommand>
{
    public async Task Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && u.Role == UserRole.Customer, ct)
            ?? throw new NotFoundException("Customer not found.");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);

        await cuckooFilter.RemoveAsync(user.Email!, UserRole.Customer);
    }
}
