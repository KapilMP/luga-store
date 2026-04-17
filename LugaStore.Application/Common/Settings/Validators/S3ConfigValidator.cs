using FluentValidation;

namespace LugaStore.Application.Common.Settings.Validators;

public class S3ConfigValidator : AbstractValidator<S3Config>
{
    public S3ConfigValidator()
    {
        RuleFor(x => x.BucketName).NotEmpty();
        RuleFor(x => x.AccessKey).NotEmpty();
        RuleFor(x => x.SecretKey).NotEmpty();
        RuleFor(x => x.Region).NotEmpty();
    }
}
