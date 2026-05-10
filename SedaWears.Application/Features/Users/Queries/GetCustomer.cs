using SedaWears.Application.Features.Users.Models;
using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users.Queries;

public record GetCustomerQuery(int Id) : IRequest<CustomerRepresentation>;

public class GetCustomerHandler(IUserService userService) :
    IRequestHandler<GetCustomerQuery, CustomerRepresentation>
{
    public async Task<CustomerRepresentation> Handle(GetCustomerQuery request, CancellationToken ct)
    {
        return await userService.GetUserByIdAndRoleAsync<CustomerRepresentation>(
            request.Id,
            UserRole.Customer,
            ct);
    }
}
