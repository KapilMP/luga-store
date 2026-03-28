using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Cart.Commands;

public record AddToCartCommand(int UserId, int ProductId, ProductSize Size, int Quantity) : IRequest;

public class AddToCartCommandHandler(ICartService cartService) : IRequestHandler<AddToCartCommand>
{
    public async Task Handle(AddToCartCommand request, CancellationToken ct)
        => await cartService.AddToCartAsync(request.UserId, request.ProductId, request.Size, request.Quantity, ct);
}

public record UpdateCartItemCommand(int ItemId, int UserId, ProductSize Size, int Quantity) : IRequest<bool>;

public class UpdateCartItemCommandHandler(ICartService cartService) : IRequestHandler<UpdateCartItemCommand, bool>
{
    public async Task<bool> Handle(UpdateCartItemCommand request, CancellationToken ct)
        => await cartService.UpdateCartItemAsync(request.ItemId, request.UserId, request.Size, request.Quantity, ct);
}

public record RemoveCartItemCommand(int ItemId, int UserId) : IRequest<bool>;

public class RemoveCartItemCommandHandler(ICartService cartService) : IRequestHandler<RemoveCartItemCommand, bool>
{
    public async Task<bool> Handle(RemoveCartItemCommand request, CancellationToken ct)
        => await cartService.RemoveCartItemAsync(request.ItemId, request.UserId, ct);
}

public record ClearCartCommand(int UserId) : IRequest;

public class ClearCartCommandHandler(ICartService cartService) : IRequestHandler<ClearCartCommand>
{
    public async Task Handle(ClearCartCommand request, CancellationToken ct)
        => await cartService.ClearCartAsync(request.UserId, ct);
}
