using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Queries;

// Admin Queries
public record GetInvitedAdminsQuery : IRequest<List<AdminProfileDto>>;
public record GetInvitedPartnersQuery : IRequest<List<PartnerProfileDto>>;
public record GetInvitedPartnerManagersQuery(int PartnerId) : IRequest<List<PartnerManagerProfileDto>>;

// Partner Queries
public record GetInvitedManagersQuery : IRequest<List<PartnerManagerProfileDto>>;

public class InvitedMemberQueryHandlers(IPartnerService partnerService, IUserService userService) :
    IRequestHandler<GetInvitedAdminsQuery, List<AdminProfileDto>>,
    IRequestHandler<GetInvitedPartnersQuery, List<PartnerProfileDto>>,
    IRequestHandler<GetInvitedPartnerManagersQuery, List<PartnerManagerProfileDto>>,
    IRequestHandler<GetInvitedManagersQuery, List<PartnerManagerProfileDto>>
{
    public Task<List<AdminProfileDto>> Handle(GetInvitedAdminsQuery request, CancellationToken cancellationToken)
        => userService.GetInvitedAdminsAsync();

    public Task<List<PartnerProfileDto>> Handle(GetInvitedPartnersQuery request, CancellationToken cancellationToken)
        => partnerService.GetInvitedPartnersAsync();

    public Task<List<PartnerManagerProfileDto>> Handle(GetInvitedPartnerManagersQuery request, CancellationToken cancellationToken)
        => partnerService.GetInvitedPartnerManagersByPartnerIdAsync(request.PartnerId);

    public Task<List<PartnerManagerProfileDto>> Handle(GetInvitedManagersQuery request, CancellationToken cancellationToken)
        => partnerService.GetInvitedManagersAsync();
}
