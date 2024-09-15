using MediatR;

namespace Common.Cqrs;

public interface IQuery<out TResponce>
    : IRequest<TResponce> 
    where TResponce : notnull
{
}
