using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Features.Orders.Commands;

public record CheckoutAddressDto(string FullName, string Phone, string Street, string City, string ZipCode);
public record CheckoutItemDto(int ProductId, int Quantity);
public record CheckoutCommand(
    int? UserId,
    string? CustomerEmail,
    CheckoutAddressDto? ShippingAddress,
    List<CheckoutItemDto> Items) : ICommand<CheckoutResult>;

public record CheckoutResult(int OrderId, string Status, decimal Total);


