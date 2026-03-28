using LugaStore.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LugaStore.WebAPI.Controllers;

[ApiController]
public abstract class LugaStoreControllerBase : ControllerBase
{
    private ICurrentUser? _currentUser;
    protected ICurrentUser CurrentUserService => _currentUser ??= HttpContext.RequestServices.GetRequiredService<ICurrentUser>();

    protected int CurrentUserId
    {
        get
        {
            var userIdStr = CurrentUserService.UserId;
            return string.IsNullOrEmpty(userIdStr) ? 0 : int.Parse(userIdStr);
        }
    }
}
