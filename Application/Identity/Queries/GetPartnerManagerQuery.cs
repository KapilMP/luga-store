using MediatR;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Queries;

public record GetPartnerManagerQuery(int Id) : IRequest<PartnerManagerProfileDto>;
public record GetPartnerManagersQuery : IRequest<List<PartnerManagerProfileDto>>;

public class GetPartnerManagerQueryHandler(IUserService userService) : IRequestHandler<GetPartnerManagerQuery, PartnerManagerProfileDto>
{
    public async Task<PartnerManagerProfileDto> Handle(GetPartnerManagerQuery request, CancellationToken cancellationToken)
        => await userService.GetPartnerManagerAsync(request.Id) ?? throw new NotFoundException("Partner manager not found.");
}

public class GetPartnerManagersQueryHandler(IUserService userService) : IRequestHandler<GetPartnerManagersQuery, List<PartnerManagerProfileDto>>
{
    public async Task<List<PartnerManagerProfileDto>> Handle(GetPartnerManagersQuery request, CancellationToken cancellationToken)
        => await userService.GetPartnerManagersAsync();
}
