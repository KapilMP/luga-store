using MediatR;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Queries;

public record GetAdminQuery(int Id) : IRequest<AdminProfileDto>;
public record GetAdminsQuery : IRequest<List<AdminProfileDto>>;

public class GetAdminQueryHandler(IUserService userService) : IRequestHandler<GetAdminQuery, AdminProfileDto>
{
    public async Task<AdminProfileDto> Handle(GetAdminQuery request, CancellationToken cancellationToken)
        => await userService.GetAdminAsync(request.Id) ?? throw new NotFoundError("Admin not found.");
}

public class GetAdminsQueryHandler(IUserService userService) : IRequestHandler<GetAdminsQuery, List<AdminProfileDto>>
{
    public async Task<List<AdminProfileDto>> Handle(GetAdminsQuery request, CancellationToken cancellationToken)
        => await userService.GetAdminsAsync();
}
