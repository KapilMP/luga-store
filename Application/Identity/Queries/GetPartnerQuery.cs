using MediatR;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Queries;

public record GetPartnerQuery(int Id) : IRequest<PartnerProfileDto>;
public record GetPartnersQuery : IRequest<List<PartnerProfileDto>>;

public class GetPartnerQueryHandler(IPartnerService partnerService) : IRequestHandler<GetPartnerQuery, PartnerProfileDto>
{
    public async Task<PartnerProfileDto> Handle(GetPartnerQuery request, CancellationToken cancellationToken)
        => await partnerService.GetPartnerByPartnerIdAsync(request.Id) ?? throw new NotFoundError("Partner not found.");
}

public class GetPartnersQueryHandler(IPartnerService partnerService) : IRequestHandler<GetPartnersQuery, List<PartnerProfileDto>>
{
    public async Task<List<PartnerProfileDto>> Handle(GetPartnersQuery request, CancellationToken cancellationToken)
        => await partnerService.GetPartnersAsync();
}
