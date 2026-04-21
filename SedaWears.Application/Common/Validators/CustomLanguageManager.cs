using FluentValidation.Resources;

namespace SedaWears.Application.Common.Validators;

public class CustomLanguageManager : LanguageManager
{
    public CustomLanguageManager()
    {
        AddTranslation("en", "NotEmptyValidator", "This field is required.");
        AddTranslation("en", "EmailValidator", "Please provide a valid email address.");
        AddTranslation("en", "AspNetCoreCompatibleEmailValidator", "Please provide a valid email address.");
    }
}
