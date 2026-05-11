using SedaWears.Application.Features.Users.Models;
using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users.Queries;

public record GetCustomerQuery(int Id) : IRequest<CustomerDto>;

public class GetCustomerHandler(IUserService userService) :
    IRequestHandler<GetCustomerQuery, CustomerDto>
{
    public async Task<CustomerDto> Handle(GetCustomerQuery request, CancellationToken ct)
    {
        return await userService.GetUserByIdAndRoleAsync<CustomerDto>(
            request.Id,
            UserRole.Customer,
            ct);
    }
}
