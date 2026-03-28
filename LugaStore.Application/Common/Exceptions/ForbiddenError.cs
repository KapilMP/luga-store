namespace LugaStore.Application.Common.Exceptions;

public class ForbiddenError(string message) : Exception(message);
