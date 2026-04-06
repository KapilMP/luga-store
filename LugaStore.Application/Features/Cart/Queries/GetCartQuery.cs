using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Cart.Queries;

public record CartItemDto(int Id, int ProductId, string Name, decimal Price, string Size, int Quantity, decimal Subtotal);

public record GetCartQuery(int UserId) : IQuery<List<CartItemDto>>;

