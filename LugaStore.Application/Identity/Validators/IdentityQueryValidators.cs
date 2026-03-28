using FluentValidation;
using LugaStore.Application.Identity.Queries;

namespace LugaStore.Application.Identity.Validators;

public class GetAdminQueryValidator : AbstractValidator<GetAdminQuery>
{
    public GetAdminQueryValidator() => RuleFor(v => v.Id).GreaterThan(0);
}

public class GetCustomerQueryValidator : AbstractValidator<GetCustomerQuery>
{
    public GetCustomerQueryValidator() => RuleFor(v => v.Id).GreaterThan(0);
}

public class GetPartnerQueryValidator : AbstractValidator<GetPartnerQuery>
{
    public GetPartnerQueryValidator() => RuleFor(v => v.Id).GreaterThan(0);
}

public class GetPartnerManagerQueryValidator : AbstractValidator<GetPartnerManagerQuery>
{
    public GetPartnerManagerQueryValidator()
    {
        RuleFor(v => v.PartnerId).GreaterThan(0);
        RuleFor(v => v.ManagerId).GreaterThan(0);
    }
}

public class GetPartnerManagersQueryValidator : AbstractValidator<GetPartnerManagersQuery>
{
    public GetPartnerManagersQueryValidator() => RuleFor(v => v.PartnerId).GreaterThan(0);
}

public class GetInvitedPartnerManagersQueryValidator : AbstractValidator<GetInvitedPartnerManagersQuery>
{
    public GetInvitedPartnerManagersQueryValidator() => RuleFor(v => v.PartnerId).GreaterThan(0);
}
