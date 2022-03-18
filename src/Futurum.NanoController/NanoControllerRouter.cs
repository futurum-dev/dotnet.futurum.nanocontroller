using System.Net;

using Futurum.Core.Result;
using Futurum.Microsoft.Extensions.DependencyInjection;
using Futurum.NanoController.Internal;

namespace Futurum.NanoController;

public interface INanoControllerRouter
{
    Task<Result<TResponse>> ExecuteAsync<TResponse>(INanoControllerRequest<TResponse> request, CancellationToken cancellationToken = default);
}

internal class NanoControllerRouter : INanoControllerRouter
{
    private readonly INanoControllerLogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMetadataCache _metadataCache;

    public NanoControllerRouter(INanoControllerLogger logger,
                                IServiceProvider serviceProvider,
                                IMetadataCache metadataCache)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _metadataCache = metadataCache;
    }

    public async Task<Result<TResponse>> ExecuteAsync<TResponse>(INanoControllerRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await CreateDispatcher(request).ThenAsync(dispatcher => dispatcher.ExecuteAsync(request, cancellationToken))
                                                  .DoWhenFailureAsync(error => _logger.Error<INanoControllerRequest<TResponse>, TResponse>(request, error));
        }
        catch (Exception exception)
        {
            _logger.UnhandledError<INanoControllerRequest<TResponse>, TResponse>(request, exception);

            return Result.Fail<TResponse>(HttpStatusCode.InternalServerError.ToResultError(exception.ToResultError()));
        }
    }

    private Result<INanoControllerDispatcher<TResponse>> CreateDispatcher<TResponse>(INanoControllerRequest<TResponse> request) =>
        _metadataCache.Get(request)
                      .Then(type => _serviceProvider.TryGetService<INanoControllerDispatcher<TResponse>>(type));
}