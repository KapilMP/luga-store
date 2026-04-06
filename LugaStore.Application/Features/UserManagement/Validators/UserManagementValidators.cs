using FluentValidation;
using LugaStore.Application.Features.UserManagement.Commands;

namespace LugaStore.Application.Features.UserManagement.Validators;

public class ActivateAdminCommandValidator : AbstractValidator<ActivateAdminCommand>
{
    public ActivateAdminCommandValidator() => RuleFor(v => v.UserId).GreaterThan(0);
}

public class DeactivateAdminCommandValidator : AbstractValidator<DeactivateAdminCommand>
{
    public DeactivateAdminCommandValidator() => RuleFor(v => v.UserId).GreaterThan(0);
}

public class DeleteAdminCommandValidator : AbstractValidator<DeleteAdminCommand>
{
    public DeleteAdminCommandValidator() => RuleFor(v => v.UserId).GreaterThan(0);
}

public class DeletePartnerCommandValidator : AbstractValidator<DeletePartnerCommand>
{
    public DeletePartnerCommandValidator() => RuleFor(v => v.PartnerId).GreaterThan(0);
}

public class ActivatePartnerCommandValidator : AbstractValidator<ActivatePartnerCommand>
{
    public ActivatePartnerCommandValidator() => RuleFor(v => v.PartnerId).GreaterThan(0);
}

public class DeactivatePartnerCommandValidator : AbstractValidator<DeactivatePartnerCommand>
{
    public DeactivatePartnerCommandValidator() => RuleFor(v => v.PartnerId).GreaterThan(0);
}

public class ActivatePartnerManagerCommandValidator : AbstractValidator<ActivatePartnerManagerCommand>
{
    public ActivatePartnerManagerCommandValidator() 
    {
        RuleFor(v => v.PartnerId).GreaterThan(0);
        RuleFor(v => v.ManagerId).GreaterThan(0);
    }
}

public class DeactivatePartnerManagerCommandValidator : AbstractValidator<DeactivatePartnerManagerCommand>
{
    public DeactivatePartnerManagerCommandValidator()
    {
        RuleFor(v => v.PartnerId).GreaterThan(0);
        RuleFor(v => v.ManagerId).GreaterThan(0);
    }
}

public class DeletePartnerManagerCommandValidator : AbstractValidator<DeletePartnerManagerCommand>
{
    public DeletePartnerManagerCommandValidator()
    {
        RuleFor(v => v.PartnerId).GreaterThan(0);
        RuleFor(v => v.ManagerId).GreaterThan(0);
    }
}

