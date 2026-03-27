using MediatR;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Queries;

public record GetPartnerQuery(int Id) : IRequest<PartnerProfileDto>;
public record GetPartnersQuery : IRequest<List<PartnerProfileDto>>;

public class GetPartnerQueryHandler(IUserService userService) : IRequestHandler<GetPartnerQuery, PartnerProfileDto>
{
    public async Task<PartnerProfileDto> Handle(GetPartnerQuery request, CancellationToken cancellationToken)
        => await userService.GetPartnerAsync(request.Id) ?? throw new NotFoundException("Partner not found.");
}

public class GetPartnersQueryHandler(IUserService userService) : IRequestHandler<GetPartnersQuery, List<PartnerProfileDto>>
{
    public async Task<List<PartnerProfileDto>> Handle(GetPartnersQuery request, CancellationToken cancellationToken)
        => await userService.GetPartnersAsync();
}
