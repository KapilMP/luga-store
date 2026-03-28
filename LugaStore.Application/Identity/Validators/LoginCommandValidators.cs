using FluentValidation;
using LugaStore.Application.Identity.Commands;

namespace LugaStore.Application.Identity.Validators;

public class LoginCommandValidator<T> : AbstractValidator<T> where T : LoginCommand
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
        RuleFor(v => v.Password).NotEmpty();
    }
}

public class CustomerLoginCommandValidator : LoginCommandValidator<CustomerLoginCommand>;
public class AdminLoginCommandValidator : LoginCommandValidator<AdminLoginCommand>;
public class PartnerLoginCommandValidator : LoginCommandValidator<PartnerLoginCommand>;
public class PartnerManagerLoginCommandValidator : LoginCommandValidator<PartnerManagerLoginCommand>;
