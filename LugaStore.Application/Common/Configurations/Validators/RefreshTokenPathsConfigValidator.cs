using FluentValidation;

namespace LugaStore.Application.Common.Configurations.Validators;

public class RefreshTokenPathsConfigValidator : AbstractValidator<RefreshTokenPathsConfig>
{
    public RefreshTokenPathsConfigValidator()
    {
        RuleFor(x => x.CustomerRefreshPath).NotEmpty().WithMessage("This field is required");
        RuleFor(x => x.AdminRefreshPath).NotEmpty().WithMessage("This field is required");
        RuleFor(x => x.PartnerRefreshPath).NotEmpty().WithMessage("This field is required");
        RuleFor(x => x.PartnerManagerRefreshPath).NotEmpty().WithMessage("This field is required");
    }
}
