using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController;

public static partial class NanoController
{
    public static class NoRequest
    {
        public abstract class Response<TResponse>
        {
            public abstract class Get : Base
            {
                protected Get(INanoControllerRouter router)
                    : base(router)
                {
                }

                [HttpGet(NanoControllerRoute.VersionedNamespace)]
                public abstract Task<ActionResult<TResponse>> GetAsync(CancellationToken cancellationToken = default);
            }
        }
    }
}