using FluentValidation;

namespace SedaWears.Application.Common.Settings.Validators;

public class S3ConfigValidator : AbstractValidator<S3Config>
{
    public S3ConfigValidator()
    {
        RuleFor(x => x.BucketName)
            .NotEmpty().WithMessage("The S3 Bucket Name is missing. Ensure 'S3:BucketName' is set.");
            
        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("The S3 Region is missing. Ensure 'S3:Region' is set.");
            
        RuleFor(x => x.AccessKey)
            .NotEmpty().WithMessage("The S3 Access Key is missing. Ensure 'S3:AccessKey' is set.");
            
        RuleFor(x => x.SecretKey)
            .NotEmpty().WithMessage("The S3 Secret Key is missing. Ensure 'S3:SecretKey' is set.");
    }
}
