namespace Futurum.NanoController;

public static partial class NanoController
{
    public abstract class Command<TCommand> : Request<TCommand>.NoResponse
    {
    }

    public abstract class Command<TCommand, TResponse> : Request<TCommand>.Response<TResponse>
    {
    }
}