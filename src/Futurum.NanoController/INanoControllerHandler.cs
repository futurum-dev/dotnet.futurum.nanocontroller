using Futurum.Core.Functional;
using Futurum.Core.Result;

namespace Futurum.NanoController;

public interface INanoControllerHandler<in TRequest, TResponse>
    where TRequest : INanoControllerRequest<TResponse>
{
    Task<Result<TResponse>> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}

public interface INanoControllerHandler<in TRequest> : INanoControllerHandler<TRequest, Unit>
    where TRequest : INanoControllerRequest<Unit>
{
}