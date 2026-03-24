using System;

namespace LugaStore.Application.Common.Exceptions;

public class NotFoundError : Exception
{
    public NotFoundError(string message) : base(message) { }
}
