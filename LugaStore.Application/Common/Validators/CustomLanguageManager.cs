using FluentValidation.Resources;

namespace LugaStore.Application.Common.Validators;

public class CustomLanguageManager : LanguageManager
{
    public CustomLanguageManager()
    {
        AddTranslation("en", "NotEmptyValidator", "This field is required.");
        AddTranslation("en", "EmailValidator", "Please provide a valid email address.");
        AddTranslation("en", "AspNetCoreCompatibleEmailValidator", "Please provide a valid email address.");
    }
}
