using Futurum.Core.Functional;

namespace Futurum.NanoController;

public interface INanoControllerRequest<TResponse>
{
}

public interface INanoControllerRequest : INanoControllerRequest<Unit>
{
}