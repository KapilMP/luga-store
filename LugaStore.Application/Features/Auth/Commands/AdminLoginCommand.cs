using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Application.Features.UserManagement.Models;


namespace LugaStore.Application.Features.Auth.Commands;

public record AdminLoginCommand(string Email, string Password) : LoginCommand(Email, Password), ICommand<(AdminAuthResponse<AdminRepresentation> Response, string RefreshToken)>;
