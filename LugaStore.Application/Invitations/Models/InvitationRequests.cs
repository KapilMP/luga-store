namespace LugaStore.Application.Invitations.Models;

public record InviteAdminRequest(string Email);
public record InvitePartnerRequest(string Email);
public record InviteManagerRequest(string Email);
