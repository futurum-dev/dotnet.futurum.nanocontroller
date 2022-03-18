namespace Futurum.NanoController;

public static partial class NanoController
{
    public static class UploadFiles
    {
        public abstract class Response<TResponse> : Request<IEnumerable<IFormFile>>.Response<TResponse>
        {
        }

        public abstract class NoResponse : Request<IEnumerable<IFormFile>>.NoResponse
        {
        }
    }
}