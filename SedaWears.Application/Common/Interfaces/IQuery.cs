using MediatR;

namespace SedaWears.Application.Common.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse> { }
