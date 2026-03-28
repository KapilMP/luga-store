using MediatR;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Queries;

public record GetCustomerQuery(int Id) : IRequest<CustomerProfileDto>;
public record GetCustomersQuery : IRequest<List<CustomerProfileDto>>;

public class GetCustomerQueryHandler(IUserService userService) : IRequestHandler<GetCustomerQuery, CustomerProfileDto>
{
    public async Task<CustomerProfileDto> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        => await userService.GetCustomerAsync(request.Id) ?? throw new NotFoundError("Customer not found.");
}

public class GetCustomersQueryHandler(IUserService userService) : IRequestHandler<GetCustomersQuery, List<CustomerProfileDto>>
{
    public async Task<List<CustomerProfileDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        => await userService.GetCustomersAsync();
}
