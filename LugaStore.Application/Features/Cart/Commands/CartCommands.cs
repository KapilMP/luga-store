using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Features.Cart.Commands;

public record AddToCartCommand(int UserId, int ProductId, ProductSize Size, int Quantity) : ICommand;

public record UpdateCartItemCommand(int ItemId, int UserId, ProductSize Size, int Quantity) : ICommand;

public record RemoveCartItemCommand(int ItemId, int UserId) : ICommand;

public record ClearCartCommand(int UserId) : ICommand;
