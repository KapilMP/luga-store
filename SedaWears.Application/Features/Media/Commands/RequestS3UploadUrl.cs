using SedaWears.Application.Common.Interfaces;
using MediatR;
using FluentValidation;

namespace SedaWears.Application.Features.Media.Commands;

public record RequestS3UploadUrlCommand(string ContentType, string FileName) : IRequest<Uri>;

public class RequestS3UploadUrlValidator : AbstractValidator<RequestS3UploadUrlCommand>
{
    public RequestS3UploadUrlValidator()
    {
        RuleFor(v => v.ContentType)
            .NotEmpty()
            .Must(ct => ct.StartsWith("image/"))
            .WithMessage("Only image content types are allowed.");

        RuleFor(v => v.FileName)
            .NotEmpty()
            .WithMessage("File name is required.");
    }
}

public class RequestS3UploadUrlHandler(IS3Service s3Service) : IRequestHandler<RequestS3UploadUrlCommand, Uri>
{
    public Task<Uri> Handle(RequestS3UploadUrlCommand request, CancellationToken cancellationToken)
    {
        var response = s3Service.GetPreSignedUrl(request.ContentType, request.FileName);
        return Task.FromResult(response);
    }
}
