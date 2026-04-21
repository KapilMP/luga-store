using FluentValidation;

namespace SedaWears.Application.Common.Settings.Validators;

public class RefreshTokenPathsConfigValidator : AbstractValidator<RefreshTokenPathsConfig>
{
    public RefreshTokenPathsConfigValidator()
    {
        RuleFor(x => x.CustomerRefreshPath)
            .NotEmpty().WithMessage("The Customer refresh token path is missing. Ensure 'RefreshTokenPaths:CustomerRefreshPath' is set.");
            
        RuleFor(x => x.AdminRefreshPath)
            .NotEmpty().WithMessage("The Admin refresh token path is missing. Ensure 'RefreshTokenPaths:AdminRefreshPath' is set.");
            
        RuleFor(x => x.OwnerRefreshPath)
            .NotEmpty().WithMessage("The Owner refresh token path is missing. Ensure 'RefreshTokenPaths:OwnerRefreshPath' is set.");
            
        RuleFor(x => x.ManagerRefreshPath)
            .NotEmpty().WithMessage("The Manager refresh token path is missing. Ensure 'RefreshTokenPaths:ManagerRefreshPath' is set.");
    }
}
