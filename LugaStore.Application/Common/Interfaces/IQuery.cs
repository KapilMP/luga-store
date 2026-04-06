using MediatR;

namespace LugaStore.Application.Common.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse> { }
