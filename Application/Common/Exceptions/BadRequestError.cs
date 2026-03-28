namespace LugaStore.Application.Common.Exceptions;

public class BadRequestError(string message) : Exception(message);
