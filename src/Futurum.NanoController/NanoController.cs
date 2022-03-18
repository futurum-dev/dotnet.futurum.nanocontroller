using Microsoft.AspNetCore.Mvc;

namespace Futurum.NanoController;

public static partial class NanoController
{
    [ApiController]
    public abstract class Base : ControllerBase
    {
        protected Base(INanoControllerRouter router)
        {
            Router = router;
        }

        protected INanoControllerRouter Router { get; }
    }
}