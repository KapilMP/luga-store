using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Products.Commands;

public record SubscribeRestockCommand(int ProductId, string Email, string Size) : IRequest<Unit>;

public class SubscribeRestockValidator : AbstractValidator<SubscribeRestockCommand>
{
    public SubscribeRestockValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Size).NotEmpty();
    }
}

public class SubscribeRestockHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<SubscribeRestockCommand, Unit>
{
    public async Task<Unit> Handle(SubscribeRestockCommand request, CancellationToken ct)
    {
        _ = await dbContext.Products.FindAsync([request.ProductId], ct)
            ?? throw new NotFoundException("Product not found");

        var exists = await dbContext.RestockSubscriptions
            .AnyAsync(s => s.ProductId == request.ProductId && s.Email == request.Email && s.Size == request.Size && !s.Notified, ct);

        if (exists) return Unit.Value;

        var sub = new RestockSubscription
        {
            ProductId = request.ProductId,
            Email = request.Email,
            Size = request.Size,
            UserId = currentUser.Id
        };

        dbContext.RestockSubscriptions.Add(sub);
        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
