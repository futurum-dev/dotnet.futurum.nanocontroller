using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController;

public static partial class NanoController
{
    public abstract class Query<TResponse> : NoRequest.Response<TResponse>.Get
    {
        protected Query(INanoControllerRouter router) : base(router)
        {
        }
    }

    public abstract class Query<TRequest, TResponse> : Request<TRequest>.Response<TResponse>.Get
    {
        protected Query(INanoControllerRouter router) : base(router)
        {
        }
    }

    public abstract class QueryById<TId, TResponse> : Base
    {
        protected QueryById(INanoControllerRouter router)
            : base(router)
        {
        }

        [HttpGet($"{NanoControllerRoute.VersionedNamespace}/{{id}}")]
        public abstract Task<ActionResult<TResponse>> GetAsync(TId id, CancellationToken cancellationToken = default);
    }
}