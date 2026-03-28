using MediatR;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Queries;

public record GetPartnerManagerQuery(int PartnerId, int ManagerId) : IRequest<PartnerManagerProfileDto>;
public record GetPartnerManagersQuery(int PartnerId) : IRequest<List<PartnerManagerProfileDto>>;
public record GetMyPartnerManagersQuery : IRequest<List<PartnerManagerProfileDto>>;

public class GetPartnerManagerQueryHandler(IPartnerService partnerService) : IRequestHandler<GetPartnerManagerQuery, PartnerManagerProfileDto>
{
    public Task<PartnerManagerProfileDto> Handle(GetPartnerManagerQuery request, CancellationToken cancellationToken)
        => partnerService.GetPartnerManagerByPartnerIdAndManagerIdAsync(request.PartnerId, request.ManagerId);
}

public class GetPartnerManagersQueryHandler(IPartnerService partnerService) : IRequestHandler<GetPartnerManagersQuery, List<PartnerManagerProfileDto>>
{
    public Task<List<PartnerManagerProfileDto>> Handle(GetPartnerManagersQuery request, CancellationToken cancellationToken)
        => partnerService.GetPartnerManagersByPartnerIdAsync(request.PartnerId);
}

public class GetMyPartnerManagersQueryHandler(IPartnerService partnerService) : IRequestHandler<GetMyPartnerManagersQuery, List<PartnerManagerProfileDto>>
{
    public Task<List<PartnerManagerProfileDto>> Handle(GetMyPartnerManagersQuery request, CancellationToken cancellationToken)
        => partnerService.GetManagersAsync();
}
