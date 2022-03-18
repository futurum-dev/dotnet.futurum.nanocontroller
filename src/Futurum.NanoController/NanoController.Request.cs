using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController;

public static partial class NanoController
{
    public static class Request<TRequest>
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
                public abstract Task<ActionResult<TResponse>> GetAsync(TRequest request, CancellationToken cancellationToken = default);
            }

            public abstract class Post : Base
            {
                protected Post(INanoControllerRouter router)
                    : base(router)
                {
                }

                [HttpPost(NanoControllerRoute.VersionedNamespace)]
                public abstract Task<ActionResult<TResponse>> PostAsync(TRequest request, CancellationToken cancellationToken = default);
            }

            public abstract class Put : Base
            {
                protected Put(INanoControllerRouter router)
                    : base(router)
                {
                }

                [HttpPut(NanoControllerRoute.VersionedNamespace)]
                public abstract Task<ActionResult<TResponse>> PutAsync(TRequest request, CancellationToken cancellationToken = default);
            }
        }

        public abstract class NoResponse
        {
            public abstract class Post : Base
            {
                protected Post(INanoControllerRouter router)
                    : base(router)
                {
                }

                [HttpPost(NanoControllerRoute.VersionedNamespace)]
                public abstract Task<IActionResult> PostAsync(TRequest request, CancellationToken cancellationToken = default);
            }

            public abstract class Delete : Base
            {
                protected Delete(INanoControllerRouter router)
                    : base(router)
                {
                }

                [HttpDelete(NanoControllerRoute.VersionedNamespace)]
                public abstract Task<IActionResult> DeleteAsync(TRequest request, CancellationToken cancellationToken = default);
            }
        }
    }
}