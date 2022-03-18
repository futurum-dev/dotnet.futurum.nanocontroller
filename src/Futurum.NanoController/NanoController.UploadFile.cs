namespace Futurum.NanoController;

public static partial class NanoController
{
    public static class UploadFile
    {
        public abstract class Response<TResponse> : Request<IFormFile>.Response<TResponse>
        {
        }

        public abstract class NoResponse : Request<IFormFile>.NoResponse
        {
        }
    }
}