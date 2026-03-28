namespace LugaStore.Application.Common.Exceptions;

public class UnauthorizedError(string message) : Exception(message);
